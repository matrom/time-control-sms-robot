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
        public static EventWaitHandle newMessageInOutbox = new AutoResetEvent(false);
        SummaryView summaryView;
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
        }
        public void Dispose()
        {
            lock (messageStorageModel.stopThreadSynch)
                messageStorageModel.stopThread = true;
            lock (summaryController.stopThreadSynch)
                summaryController.stopThread = true;
        }
    }
}
