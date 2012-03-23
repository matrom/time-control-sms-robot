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
        private List<string> _log = new List<string>();
        public List<string> Log
        {
            get { return _log; }
            set
            {
                _log = value;
                listBoxOutbox.Items.Clear();
                foreach (string str in _log)
                    listBoxOutbox.Items.Add(str);
            }
        }
        ThreadManager threadManager;
        public SummaryView()
        {
            threadManager = new ThreadManager(this);

            InitializeComponent();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            lock (this.messageToSend)
            {
                this.messageToSend.number = textBoxNumber.Text;
                this.messageToSend.text = textBoxMessageText.Text;
            }
            ThreadManager.newMessageByUser.Set();
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
