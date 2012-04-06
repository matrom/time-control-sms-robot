using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class MessageStorageModel
    {
        public List<Message> Inbox = new List<Message>();
        public List<Message> Outbox = new List<Message>();
        public object stopThreadSynch = new object();
        public bool stopThread = false;
        public DatabaseManager databaseManager = new DatabaseManager();
        public MessageStorageModel()
        {

        }
        /*public void run()
        {
            bool localStop = false;
            while (!localStop)
            {
                List<Message> InboxCashe = new List<Message>();
                List<Message> OutboxCashe = new List<Message>();
                bool messageProcessed = false;
                lock (Inbox)
                    foreach(Message mes in Inbox)
                        if (!mes.isProcessed)
                        {
                            InboxCashe.Add(new Message(mes));
                            mes.isProcessed = true;
                            messageProcessed = true;
                        }
                if (messageProcessed)
                {
                    ThreadManager.newMessageInInbox.Set();
                    foreach (Message mes in InboxCashe)
                        OutboxCashe.Add(processMessage(mes));
                    lock (Outbox)
                        foreach (Message mes in OutboxCashe)
                            Outbox.Add(mes);
                    ThreadManager.newMessageInOutbox.Set();
                    processOutbox();
                }
                lock (stopThreadSynch)
                    localStop = stopThread;
            }
        }*/

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
                     databaseManager.LogMessage(mes);
                     // DEBUG: Imitation of business logic processing
                     Thread.Sleep(2000);
                     Message reply = processMessage(mes);
                     mes.isProcessed = true;
                     databaseManager.LogMessage(mes);
                     lock (Inbox)
                         foreach (Message m in Inbox)
                             if (m.id == mes.id)
                                 m.isProcessed = true;
                     ThreadManager.newMessageInInbox.Set();
                     
                     lock (Outbox)
                         Outbox.Add(reply);
                     //ThreadManager.newMessageInOutbox.Set();
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
                     databaseManager.LogMessage(mes);
                     // DEBUG: Imitation of message send
                     Thread.Sleep(2000);
                     mes.isProcessed = true;
                     databaseManager.LogMessage(mes);
                     lock (Outbox)
                     {
                         foreach (Message m in Outbox)
                             if (m.id == mes.id)
                                 m.isProcessed = true;
                         //Outbox.Add(mes);
                     }
                     ThreadManager.newMessageInOutbox.Set();
                 }
             }
        }

        public Message processMessage(Message mes)
        {
            Message reply = new Message();
            //reply.CopyContents(mes);
            reply.To = mes.From;
            reply.isProcessed = false;
            reply.text = "Echo: " + mes.text;
            return reply;
        }
        /*public void processOutbox()
        {
            List<Message> OutboxCashe = new List<Message>();
            lock (Outbox)
                foreach (Message mes in Outbox)
                    if (!mes.isProcessed)
                    {
                        OutboxCashe.Add(new Message(mes));
                        mes.isProcessed = true;
                    }
        }*/
    }
}
