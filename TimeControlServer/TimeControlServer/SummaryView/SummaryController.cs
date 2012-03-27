using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    public enum messageSource { Inbox, Outbox, User, DB, SMS };
    class SummaryController: IDisposable
    {
        


        public static EventWaitHandle newMessage = new AutoResetEvent(false);
        private bool StopListeners = false;
        private Stack<messageSource> messageSources = new Stack<messageSource>();
        private Thread InboxListenerThread;
        private Thread OutboxListenerThread;
        private Thread UserListenerThread;
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
            UserListenerThread = new Thread(UserListener);
            UserListenerThread.Start();

            while (!localStop)
            {
                SummaryController.newMessage.WaitOne();
                while (messageSources.Count > 0)
                {
                    messageSource source;
                    lock (messageSources)
                        source = messageSources.Pop();



                    if (source == messageSource.User)
                    {
                        lock (model.Inbox)
                            model.Inbox.Add(new Message(summaryView.messageToSend));

                        lock (summaryView.Log)
                            //summaryView.Log.Add("Message received");
                            summaryView.AddLogMessage("New message was created by user");
                    }


                    if (source == messageSource.Inbox)
                    {

                        lock (summaryView.InboxCashe)
                        {
                            summaryView.InboxCashe.Clear();
                            foreach (Message mes in model.Inbox)
                                summaryView.InboxCashe.Add(new Message(mes));
                            summaryView.ModifyInboxOrOutbox(source);
                        }
                        lock (summaryView.Log)
                            //summaryView.Log.Add("Message received");
                            summaryView.AddLogMessage("New message in Inbox");
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
                        lock (summaryView.OutboxCashe)
                        {
                            foreach (Message mes in OutboxCashe)
                                summaryView.OutboxCashe.Add(new Message(mes));
                            summaryView.ModifyInboxOrOutbox(source);
                        }
                        /*foreach (Message mes in model.Outbox)
                            if (!mes.isProcessed)
                                OutboxCashe.Add(new Message(mes));*/
                        lock (summaryView.Log)
                            //summaryView.Log.Add("Message received");
                            summaryView.AddLogMessage("New message in Outbox");
                    }


                    // Do something
                    lock (stopThreadSynch)
                        localStop = stopThread;
                }
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
        public void UserListener()
        {
            while (!StopListeners)
            {
                ThreadManager.newMessageByUser.WaitOne();
                lock (messageSources)
                    messageSources.Push(messageSource.User);
                SummaryController.newMessage.Set();
            }
        }
        public void Dispose()
        {
            StopListeners = true;
        }
    }
}
