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
        public List<Message> OutboxCashe = new List<Message>();
        public List<Message> InboxCashe = new List<Message>();
        delegate void AddLogItemCallback(string text);
        delegate void ModifyInboxOrOutboxCallback(messageSource source);
       
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
                listBoxLog.Items.Insert(0, "["+ DateTime.Now.ToString()+ "]: "+message);
            }
        }

        public void ModifyInboxOrOutbox(messageSource source)
        {
            if (this.listBoxLog.InvokeRequired)
            {
                ModifyInboxOrOutboxCallback d = new ModifyInboxOrOutboxCallback(ModifyInboxOrOutbox);
                this.Invoke(d, new object[] { source });
            }
            else
            {
                if (source == messageSource.Inbox)
                {
                    listBoxInbox.Items.Clear();
                    foreach (Message mes in InboxCashe)
                        listBoxInbox.Items.Insert(0, mes.ToString());
                }
                else
                {
                    listBoxOutbox.Items.Clear();
                    foreach (Message mes in OutboxCashe)
                        listBoxOutbox.Items.Insert(0, mes.ToString());

                }

                //Inbox.Add(message);
                /*listBoxLog.Items.Clear();
                foreach (string str in Log)*/
                
                //foreach (
                //listBoxInbox.Items.Add(message);
            }
        }



        private void buttonSend_Click(object sender, EventArgs e)
        {
            lock (this.messageToSend)
            {
                this.messageToSend.To = textBoxNumber.Text;
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

        private void SummaryView_Load(object sender, EventArgs e)
        {

        }
    }
}
