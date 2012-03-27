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
                while (messageSources.Count > 0)
                {
                    messageSource source;
                    lock (messageSources)
                        source = messageSources.Pop();



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
                ThreadManager.newMessageInOutbox.WaitOne();
                // Process messages in Outbox by SMS sender module

                SMSManager.newSMSMessage.Set();
            }
        }
        public void SMSListener()
        {
            while (!StopListeners)
            {
                ThreadManager.newMessageBySMS.WaitOne();
                lock (messageSources)
                    messageSources.Push(messageSource.SMS);
                SMSManager.newSMSMessage.Set();
            }
        }


    }
}
