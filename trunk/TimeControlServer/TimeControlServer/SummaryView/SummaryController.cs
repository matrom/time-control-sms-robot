using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class SummaryController
    {
        public SummaryController()
        {
        }
        public object stopThreadSynch = new object();
        public bool stopThread = false;
        public MessageStorageModel model;
        public SummaryView summaryView;
        public void run()
        {
            bool localStop = false;
            while (!localStop)
            {
                ThreadManager.newMessageByUser.WaitOne();
                lock (model.Inbox)
                    model.Inbox.Add(new Message());
                lock (summaryView.Log)
                    summaryView.Log.Add("Message received");
                // Do something
                lock (stopThreadSynch)
                    localStop = stopThread;
            }
        }
    }
}
