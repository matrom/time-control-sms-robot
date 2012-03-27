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
        public static EventWaitHandle newMessageByUser = new AutoResetEvent(false);
        public static EventWaitHandle newMessageByDB = new AutoResetEvent(false);
        public static EventWaitHandle newMessageBySMS = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox = new AutoResetEvent(false);
        SummaryView summaryView;
        SMSManager smsManager;
        Thread smsManagerThread;
        public ThreadManager(SummaryView view)
        {
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
            smsManagerThread = new Thread(smsManager.run);
            smsManagerThread.Start();
            
        }
        public void Dispose()
        {
            lock (messageStorageModel.stopThreadSynch)
                messageStorageModel.stopThread = true;
            lock (summaryController.stopThreadSynch)
                summaryController.stopThread = true;
            lock (smsManager.stopThreadSynch)
                smsManager.stopThread = true;
        }
    }
}
