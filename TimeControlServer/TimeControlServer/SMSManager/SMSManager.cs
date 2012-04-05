using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TimeControlServer
{
    class SMSManager : IDisposable
    {
        public object stopThreadSynch = new object();
        SmsEmulator emulator  = new SmsEmulator();
        public bool stopThread = false;
        private bool StopListeners = false;
        private Thread emulatorThreadEnvelopeThread;
        private Thread OutboxListenerThread;
        private Thread SMSListenerThread;
        public MessageStorageModel model;
        private Stack<messageSource> messageSources = new Stack<messageSource>();
        public static EventWaitHandle newSMSMessage = new AutoResetEvent(false);
        public SMSManager()
        {
        }
        public SMSManager(MessageStorageModel model)
        {
            this.model = model;
        }
        public void run()
        {
            emulatorThreadEnvelopeThread = new Thread(emulatorThreadEnvelope);
            emulatorThreadEnvelopeThread.Start();
            OutboxListenerThread = new Thread(OutboxListener);
            OutboxListenerThread.Start();
            SMSListenerThread = new Thread(SMSListener);
            SMSListenerThread.Start();
            bool localStop = false;
            while (!localStop)
            {
                SMSManager.newSMSMessage.WaitOne();
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



                    if (source == messageSource.SMS)
                    {
                        lock (model.Inbox)
                            model.Inbox.Add(new Message(emulator.mes));
                    }


                    lock (stopThreadSynch)
                        localStop = stopThread;
                }
            }
        }
        public void Dispose()
        {
            StopListeners = true;
            emulator.Dispose();
        }
        private void emulatorThreadEnvelope()
        {
            emulator.ShowDialog();
        }
        public void OutboxListener()
        {
            while (!StopListeners)
            {

                bool success = ThreadManager.newMessageInOutbox_sms.WaitOne(5000);
                // Process messages in Outbox by SMS sender module
                if (success)
                {
                    SMSManager.newSMSMessage.Set();
                }
            }
        }
        public void SMSListener()
        {
            while (!StopListeners)
            {
                bool success = ThreadManager.newMessageBySMS.WaitOne(5000);
                if (success)
                {
                    lock (messageSources)
                        messageSources.Push(messageSource.SMS);
                    SMSManager.newSMSMessage.Set();
                }
            }
        }


    }
}
