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
        private Thread SMSListenerThread;
        public MessageStorageModel model;
        
        public SMSManager(MessageStorageModel model)
        {
            this.model = model;
            emulatorThreadEnvelopeThread = new Thread(emulatorThreadEnvelope);
            emulatorThreadEnvelopeThread.Start();
            SMSListenerThread = new Thread(SMSListener);
            SMSListenerThread.Start();
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
        
        public void SMSListener()
        {
            while (!StopListeners)
            {
                lock (emulator.justReceived)
                {
                    if (emulator.justReceived.Count > 0)
                    {
                        /*lock (model.Inbox)
                            foreach (Message mes in emulator.justReceived)
                                model.Inbox.Add(mes);*/
                        foreach (Message mes in emulator.justReceived)
                            model.addMessage(mes, "Inbox");
                        emulator.justReceived.Clear();
                    }
                }
                Thread.Sleep(3000);
            }
        }
        public void SendSms(Message mes)
        {
            emulator.sendMessage(mes);
        }

    }
}
