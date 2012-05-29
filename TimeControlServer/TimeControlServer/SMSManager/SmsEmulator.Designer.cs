namespace TimeControlServer
{
    partial class SmsEmulator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMessageText = new System.Windows.Forms.TextBox();
            this.buttonEmulateSMSReceive = new System.Windows.Forms.Button();
            this.listBoxSendMessages = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "From";
            // 
            // textBoxFrom
            // 
            this.textBoxFrom.Location = new System.Drawing.Point(184, 43);
            this.textBoxFrom.Name = "textBoxFrom";
            this.textBoxFrom.Size = new System.Drawing.Size(173, 20);
            this.textBoxFrom.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Message Text";
            // 
            // textBoxMessageText
            // 
            this.textBoxMessageText.Location = new System.Drawing.Point(165, 97);
            this.textBoxMessageText.Multiline = true;
            this.textBoxMessageText.Name = "textBoxMessageText";
            this.textBoxMessageText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessageText.Size = new System.Drawing.Size(192, 79);
            this.textBoxMessageText.TabIndex = 3;
            // 
            // buttonEmulateSMSReceive
            // 
            this.buttonEmulateSMSReceive.Location = new System.Drawing.Point(113, 226);
            this.buttonEmulateSMSReceive.Name = "buttonEmulateSMSReceive";
            this.buttonEmulateSMSReceive.Size = new System.Drawing.Size(186, 23);
            this.buttonEmulateSMSReceive.TabIndex = 4;
            this.buttonEmulateSMSReceive.Text = "Emulate SMS receive";
            this.buttonEmulateSMSReceive.UseVisualStyleBackColor = true;
            this.buttonEmulateSMSReceive.Click += new System.EventHandler(this.buttonEmulateSMSReceive_Click);
            // 
            // listBoxSendMessages
            // 
            this.listBoxSendMessages.FormattingEnabled = true;
            this.listBoxSendMessages.Location = new System.Drawing.Point(482, 77);
            this.listBoxSendMessages.Name = "listBoxSendMessages";
            this.listBoxSendMessages.ScrollAlwaysVisible = true;
            this.listBoxSendMessages.Size = new System.Drawing.Size(389, 121);
            this.listBoxSendMessages.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(540, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Send messages:";
            // 
            // SmsEmulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 318);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxSendMessages);
            this.Controls.Add(this.buttonEmulateSMSReceive);
            this.Controls.Add(this.textBoxMessageText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxFrom);
            this.Controls.Add(this.label1);
            this.Name = "SmsEmulator";
            this.Text = "SmsEmulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMessageText;
        private System.Windows.Forms.Button buttonEmulateSMSReceive;
        private System.Windows.Forms.ListBox listBoxSendMessages;
        private System.Windows.Forms.Label label3;
    }
}