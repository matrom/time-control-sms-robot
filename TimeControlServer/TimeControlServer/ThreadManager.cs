using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class ThreadManager : IDisposable 
    {
        Thread summaryControllerThread;
        SummaryController summaryController;
        public static EventWaitHandle newMessageInInbox = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInInbox_view = new AutoResetEvent(false);
        public static EventWaitHandle newMessageByDB = new AutoResetEvent(false);
        public static EventWaitHandle newMessageBySMS = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox_view = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox_sms = new AutoResetEvent(false);
        SummaryView summaryView;
        private Thread InboxListenerThread;
        private Thread OutboxListenerThread;
        private bool StopListeners = false;
        public ThreadManager(SummaryView view, MessageStorageModel model)
        {
            InboxListenerThread = new Thread(InboxListener);
            InboxListenerThread.Start();
            OutboxListenerThread = new Thread(OutboxListener);
            OutboxListenerThread.Start();
            summaryView = view;
            summaryController = new SummaryController();
            summaryController.model = model;
            summaryController.summaryView = this.summaryView;
            summaryControllerThread = new Thread(summaryController.run);
            summaryControllerThread.Start();
        }
        public void Dispose()
        {
            StopListeners = true;
            lock (summaryController.stopThreadSynch)
                summaryController.stopThread = true;
        }
        public void InboxListener()
        {
            while (!StopListeners)
            {
                bool success = ThreadManager.newMessageInInbox.WaitOne(5000);
                if (success)
                {
                    ThreadManager.newMessageInInbox_view.Set();
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
                    ThreadManager.newMessageInOutbox_view.Set();
                    ThreadManager.newMessageInOutbox_sms.Set();
                }
            }
        }


    }
}
