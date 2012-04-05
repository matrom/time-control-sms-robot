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
        public Message mes;// = new Message();
        public SmsEmulator()
        {
            InitializeComponent();
        }

        private void buttonEmulateSMSReceive_Click(object sender, EventArgs e)
        {
            mes = new Message();
            mes.From = textBoxFrom.Text;
            mes.text = textBoxMessageText.Text;
            ThreadManager.newMessageBySMS.Set();
        }
    }
}
