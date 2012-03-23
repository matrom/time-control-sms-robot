﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeControlServer
{
    class MessageStorageModel
    {
        public List<Message> Inbox = new List<Message>();
        public List<Message> Outbox = new List<Message>();
        public object stopThreadSynch = new object();
        public bool stopThread = false;
        public void run()
        {
            bool localStop = false;
            while (!localStop)
            {
                List<Message> InboxCashe = new List<Message>();
                List<Message> OutboxCashe = new List<Message>();
                lock (Inbox)
                    foreach(Message mes in Inbox)
                        if (!mes.isProcessed)
                        {
                            InboxCashe.Add(new Message(mes));
                            mes.isProcessed = true;
                        }
                foreach (Message mes in InboxCashe)
                    OutboxCashe.Add(processMessage(mes));
                lock (Outbox)
                    foreach (Message mes in OutboxCashe)
                        Outbox.Add(mes);
                // Do something
                lock (stopThreadSynch)
                    localStop = stopThread;
            }
        }
        public Message processMessage(Message mes)
        {
            mes.isProcessed = false;
            mes.text = "Echo: " + mes.text;
            return mes;
        }
    }
}