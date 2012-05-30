using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace TimeControlServer
{
    class MessageStorageModel
    {
        public List<Message> Inbox = new List<Message>();
        public List<Message> Outbox = new List<Message>();
        public object stopThreadSynch = new object();
        public bool stopThread = false;
        public DatabaseManager databaseManager;
        SMSManager smsManager;
        Thread messageStorageModelThread;
        public MessageStorageModel()
        {
            databaseManager = new DatabaseManager(Outbox);
            smsManager = new SMSManager(this);
            messageStorageModelThread = new Thread(run);
            messageStorageModelThread.Start();
        }
        public void addMessage(Message mes, string destination)
        {
            if (destination == "Outbox")
            {
                lock (Outbox)
                {
                    bool alreadyExists = false;
                    foreach (Message outboxMessage in Outbox)
                        if (outboxMessage.id == mes.id)
                            alreadyExists = true;
                    if (!alreadyExists)
                        Outbox.Add(mes);
                }
            }
            if (destination == "Inbox")
            {
                lock (Inbox)
                {
                    bool alreadyExists = false;
                    foreach (Message inboxMessage in Inbox)
                        if (inboxMessage.id == mes.id)
                            alreadyExists = true;
                    if (!alreadyExists)
                        Inbox.Add(mes);
                }
            }
        }

        public void run()
        {
             bool localStop = false;
             while (!localStop)
             {
                 int i = 0;
                 bool found = false;
                 Message mes = new Message();
                 lock(Inbox)
                     while (!found && Inbox.Count > i)
                     {
                         if (Inbox[i].isProcessed == false)
                         {
                             mes = new Message(Inbox[i]);
                             found = true;
                         }
                         i++;
                     }
                 if (found)
                 {
                     ThreadManager.newMessageInInbox.Set();
                     databaseManager.ProcessMessage(mes, "Inbox");
                     lock (Inbox)
                         foreach (Message m in Inbox)
                             if (m.id == mes.id)
                                 m.isProcessed = true;
                     ThreadManager.newMessageInInbox.Set();
                 }
                 found = false;
                 i = 0;
                 mes = new Message();
                 lock (Outbox)
                     while (!found && Outbox.Count > i)
                     {
                         if (Outbox[i].isProcessed == false)
                         {
                             mes = new Message(Outbox[i]);
                             found = true;
                         }
                         i++;
                     }
                 if (found)
                 {
                     ThreadManager.newMessageInOutbox.Set();
                     // Если письмо уже содержится в Outbox в базе данных, то ничего не произойдёт. Если же его там ещё нет, оно будет добавлено
                     databaseManager.ProcessMessage(mes, "Outbox");
                     // Call SMS sender to send message
                     smsManager.SendSms(mes);
                     databaseManager.ProcessMessage(mes, "Send");
                     lock (Outbox)
                     {
                         foreach (Message m in Outbox)
                             if (m.id == mes.id)
                                 m.isProcessed = true;
                     }
                     ThreadManager.newMessageInOutbox.Set();
                 }
             }
        }

    }
}
