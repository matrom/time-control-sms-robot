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
    public partial class SummaryView : Form
    {
        public Message messageToSend = new Message();
        public List<string> Log = new List<string>();
        delegate void AddLogItemCallback(string text);
       
        ThreadManager threadManager;
        public SummaryView()
        {
            InitializeComponent();
            threadManager = new ThreadManager(this);
        }
        public void AddLogMessage(string message)
        {
            if (this.listBoxLog.InvokeRequired)
            {
                AddLogItemCallback d = new AddLogItemCallback(AddLogMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                Log.Add(message);
                /*listBoxLog.Items.Clear();
                foreach (string str in Log)*/
                listBoxLog.Items.Add(message);
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            lock (this.messageToSend)
            {
                this.messageToSend.number = textBoxNumber.Text;
                this.messageToSend.text = textBoxMessageText.Text;
            }
            ThreadManager.newMessageInInbox.Set();
            // Debug code, need to delete
            /*MessageStorageModel model = new MessageStorageModel();
            model.Inbox.Add(this.messageToSend);
            model.stopThread = true;
            model.run();
            foreach (Message mes in model.Outbox)
                listBoxOutbox.Items.Add(mes.ToString());*/
        }
    }
}
