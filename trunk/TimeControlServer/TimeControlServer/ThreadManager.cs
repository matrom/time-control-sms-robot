using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class ThreadManager : IDisposable 
    {
        
        SummaryController summaryController;
        public static EventWaitHandle newMessageInInbox = new AutoResetEvent(false);
        public static EventWaitHandle newMessageByDB = new AutoResetEvent(false);
        public static EventWaitHandle newMessageBySMS = new AutoResetEvent(false);
        public static EventWaitHandle newMessageInOutbox = new AutoResetEvent(false);
        SummaryView summaryView;
        public ThreadManager(SummaryView view, MessageStorageModel model)
        {
            summaryView = view;
            summaryController = new SummaryController(model, view);
        }
        public void Dispose()
        {
            
        }
    }
}
