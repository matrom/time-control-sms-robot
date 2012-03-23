using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class SummaryController: IDisposable
    {
        enum messageSource { Inbox, Outbox };


        public static EventWaitHandle newMessage = new AutoResetEvent(false);
        private bool StopListeners = false;
        private Stack<messageSource> messageSources = new Stack<messageSource>();
        private Thread InboxListenerThread;
        private Thread OutboxListenerThread;
        private int OutboxItemsCount = 0;
        public SummaryController()
        {
        }
        public SummaryController(MessageStorageModel model, SummaryView summaryView)
        {
            this.model = model;
            this.summaryView = summaryView;
        }
        public object stopThreadSynch = new object();
        public bool stopThread = false;
        public MessageStorageModel model;
        public SummaryView summaryView;
        public void run()
        {
            bool localStop = false;
            InboxListenerThread = new Thread(InboxListener);
            InboxListenerThread.Start();
            OutboxListenerThread = new Thread(OutboxListener);
            OutboxListenerThread.Start();

            while (!localStop)
            {
                SummaryController.newMessage.WaitOne();
                messageSource source;
                lock (messageSources)
                    source = messageSources.Pop();

                if (source == messageSource.Inbox)
                {
                    lock (model.Inbox)
                        model.Inbox.Add(new Message(summaryView.messageToSend));
                    lock (summaryView.Log)
                        //summaryView.Log.Add("Message received");
                        summaryView.AddLogMessage("Message received");
                }
                if (source == messageSource.Outbox)
                {
                    List<Message> OutboxCashe = new List<Message>();
                    lock (model.Outbox)
                    {
                        for (int i = OutboxItemsCount; i < model.Outbox.Count; i++)
                            OutboxCashe.Add(new Message(model.Outbox[i]));
                        OutboxItemsCount = model.Outbox.Count;
                    }

                        /*foreach (Message mes in model.Outbox)
                            if (!mes.isProcessed)
                                OutboxCashe.Add(new Message(mes));*/
                        
                        

                    lock (summaryView.Log)
                        //summaryView.Log.Add("Message received");
                        foreach(Message mes in OutboxCashe)
                            summaryView.AddLogMessage(mes.ToString());
                }


                // Do something
                lock (stopThreadSynch)
                    localStop = stopThread;
            }
        }
        public void InboxListener()
        {
            while (!StopListeners)
            {
                ThreadManager.newMessageInInbox.WaitOne();
                lock (messageSources)
                    messageSources.Push(messageSource.Inbox);

                SummaryController.newMessage.Set();
            }
        }
        public void OutboxListener()
        {
            while (!StopListeners)
            {
                ThreadManager.newMessageInOutbox.WaitOne();
                lock (messageSources)
                    messageSources.Push(messageSource.Outbox);
                SummaryController.newMessage.Set();
            }
        }
        public void Dispose()
        {
            StopListeners = true;
        }
    }
}
