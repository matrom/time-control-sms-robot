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
        public List<string> Log = new List<string>();
        public BindingList<Message> OutboxCashe = new BindingList<Message>();
        public BindingList<Message> InboxCashe = new BindingList<Message>();
        delegate void AddLogItemCallback(string text);
        delegate void ModifyInboxOrOutboxCallback(messageSource source);
        MessageStorageModel messageStorageModel;
        ThreadManager threadManager;
        public SummaryView()
        {
            InitializeComponent();
            messageStorageModel = new MessageStorageModel();
            threadManager = new ThreadManager(this, messageStorageModel);
            dataGridViewOutbox.DataSource = OutboxCashe;
            dataGridViewInbox.DataSource = InboxCashe;
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
                    lock (messageStorageModel.Inbox)
                    {
                        bool alreadyExist = false;
                        foreach (Message mes in messageStorageModel.Inbox)
                        {
                            alreadyExist = false;
                            for (int i = 0; i<InboxCashe.Count; i++)
                                if (mes.id == InboxCashe[i].id)
                                {
                                    alreadyExist = true;
                                    InboxCashe[i] = mes;
                                }
                            if (!alreadyExist)
                                InboxCashe.Add(mes);
                        }
                    }
                }
                else
                {
                    lock (messageStorageModel.Outbox)
                    {
                        bool alreadyExist = false;
                        foreach (Message mes in messageStorageModel.Outbox)
                        {
                            alreadyExist = false;
                            for (int i = 0; i < OutboxCashe.Count; i++)
                                if (mes.id == OutboxCashe[i].id)
                                {
                                    alreadyExist = true;
                                    OutboxCashe[i] = mes;
                                }
                            if (!alreadyExist)
                                OutboxCashe.Add(mes);
                        }
                    }
                }
            }
        }



        private void buttonSend_Click(object sender, EventArgs e)
        {
            Message mes = new Message();
            mes.From = "Server";
            mes.To = textBoxNumber.Text;
            mes.text = textBoxMessageText.Text;
            messageStorageModel.addMessage(mes, "Outbox");
        }

        private void SummaryView_FormClosed(object sender, FormClosedEventArgs e)
        {
            //threadManager.Dispose();
            Application.Exit();
            //Environment.Exit();
        }

        private void checkBoxToAllUsers_CheckedChanged(object sender, EventArgs e)
        {
            textBoxNumber.Enabled = !checkBoxToAllUsers.Checked;
        }
    }
}
