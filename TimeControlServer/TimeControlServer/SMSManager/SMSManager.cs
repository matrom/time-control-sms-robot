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
        private Thread emulatorThreadEnvelopeThread;
        public void run()
        {
            emulatorThreadEnvelopeThread = new Thread(emulatorThreadEnvelope);
            emulatorThreadEnvelopeThread.Start();
            bool localStop = false;
            while (!localStop)
            {
                Thread.Sleep(100);
                lock (stopThreadSynch)
                    localStop = stopThread;
            }
        }
        public void Dispose()
        {
            emulator.Dispose();
        }
        private void emulatorThreadEnvelope()
        {
            emulator.ShowDialog();
        }

    }
}
