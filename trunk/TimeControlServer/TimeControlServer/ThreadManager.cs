using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class ThreadManager : IDisposable 
    {
        Thread messageStorageModelThread;
        MessageStorageModel messageStorageModel;
        Thread summaryControllerThread;
        SummaryController summaryController;
        public static EventWaitHandle newMessageInInbox = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInInbox_view = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInInbox_sms = new AutoResetEvent(false);
        public static EventWaitHandle newMessageByUser = new AutoResetEvent(false);
        public static EventWaitHandle newMessageByDB = new AutoResetEvent(false);
        public static EventWaitHandle newMessageBySMS = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox_view = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox_sms = new AutoResetEvent(false);
        SummaryView summaryView;
        SMSManager smsManager;
        Thread smsManagerThread;
        private Thread InboxListenerThread;
        private Thread OutboxListenerThread;
        private bool StopListeners = false;
        public ThreadManager(SummaryView view)
        {
            InboxListenerThread = new Thread(InboxListener);
            InboxListenerThread.Start();
            OutboxListenerThread = new Thread(OutboxListener);
            OutboxListenerThread.Start();


            summaryView = view;
            messageStorageModel = new MessageStorageModel();
            messageStorageModelThread = new Thread(messageStorageModel.run);
            messageStorageModelThread.Start();
            summaryController = new SummaryController();
            summaryController.model = messageStorageModel;
            summaryController.summaryView = this.summaryView;
            summaryControllerThread = new Thread(summaryController.run);
            summaryControllerThread.Start();
            smsManager = new SMSManager();
            smsManager.model = messageStorageModel;
            smsManagerThread = new Thread(smsManager.run);
            smsManagerThread.Start();
            
        }
        public void Dispose()
        {
            StopListeners = true;
            lock (messageStorageModel.stopThreadSynch)
                messageStorageModel.stopThread = true;
            lock (summaryController.stopThreadSynch)
                summaryController.stopThread = true;
            lock (smsManager.stopThreadSynch)
                smsManager.stopThread = true;
        }
        public void InboxListener()
        {
            while (!StopListeners)
            {
                bool success = ThreadManager.newMessageInInbox.WaitOne(5000);
                if (success)
                {
                    ThreadManager.newMessageInInbox_view.Set();
                    ThreadManager.newMessageInInbox_sms.Set();
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
