using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TimeControlServer
{
    public partial class SmsEmulator : Form
    {
        public List<Message> justReceived = new List<Message>();
        delegate void SendMessageCallback(Message mes);
        //public Message mes;// = new Message();
        public SmsEmulator()
        {
            InitializeComponent();
        }

        private void buttonEmulateSMSReceive_Click(object sender, EventArgs e)
        {
            Message mes = new Message();
            mes = new Message();
            mes.From = textBoxFrom.Text;
            mes.text = textBoxMessageText.Text;
            lock(justReceived)
                justReceived.Add(mes);
            //ThreadManager.newMessageBySMS.Set();
        }
        public void sendMessage(Message mes)
        {
            if (this.listBoxSendMessages.InvokeRequired)
            {
                SendMessageCallback d = new SendMessageCallback(sendMessage);
                this.Invoke(d, new object[] { mes });
            }
            else
            {
                listBoxSendMessages.Items.Insert(0, mes.ToString());
            }
        }

    }
}
