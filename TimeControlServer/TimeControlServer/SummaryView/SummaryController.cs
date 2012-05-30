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
        private Thread summaryControllerThread;
        public SummaryController(MessageStorageModel model, SummaryView summaryView)
        {
            this.model = model;
            this.summaryView = summaryView;
            summaryControllerThread = new Thread(run);
            summaryControllerThread.Start();
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
                int sourcesQty = 0;
                lock (messageSources)
                    sourcesQty = messageSources.Count;
                while (sourcesQty > 0)
                {
                    messageSource source;
                    lock (messageSources)
                    {
                        source = messageSources.Pop();
                        sourcesQty = messageSources.Count;
                    }
                    string logMessage = "";
                    if (source == messageSource.Inbox)
                        logMessage = "Inbox updated";
                    if (source == messageSource.Outbox)
                        logMessage = "Outbox updated";

                    summaryView.ModifyInboxOrOutbox(source);
                    lock (summaryView.Log)
                        summaryView.AddLogMessage(logMessage);
                    lock (stopThreadSynch)
                        localStop = stopThread;
                }
            }
        }
        public void InboxListener()
        {
            while (!StopListeners)
            {
                bool success = ThreadManager.newMessageInInbox.WaitOne(5000);
                if (success)
                {
                    lock (messageSources)
                        messageSources.Push(messageSource.Inbox);

                    SummaryController.newMessage.Set();
                }
            }
        }
        public void OutboxListener()
        {
            while (!StopListeners)
            {
                bool success = ThreadManager.newMessageInOutbox.WaitOne(5000);
                if (success)
                {
                    lock (messageSources)
                        messageSources.Push(messageSource.Outbox);
                    SummaryController.newMessage.Set();
                }
            }
        }
        
        public void Dispose()
        {
            lock (stopThreadSynch)
                stopThread = true;
            StopListeners = true;
        }
    }
}
