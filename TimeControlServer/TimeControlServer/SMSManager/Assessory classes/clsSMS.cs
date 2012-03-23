using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace TimeControlServer
{
    public class clsSMS
    {

        #region Open and Close Ports
        //Open Port
        public SerialPort OpenPort(string p_strPortName, int p_uBaudRate, int p_uDataBits, int p_uReadTimeout, int p_uWriteTimeout)
        {
            receiveNow = new AutoResetEvent(false);
            SerialPort port = new SerialPort();

            try
            {           
                port.PortName = p_strPortName;                 //COM1
                port.BaudRate = p_uBaudRate;                   //9600
                port.DataBits = p_uDataBits;                   //8
                port.StopBits = StopBits.One;                  //1
                port.Parity = Parity.None;                     //None
                port.ReadTimeout = p_uReadTimeout;             //300
                port.WriteTimeout = p_uWriteTimeout;           //300
                port.Encoding = Encoding.GetEncoding("iso-8859-1");
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                port.Open();
                port.DtrEnable = true;
                port.RtsEnable = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return port;
        }

        //Close Port
        public void ClosePort(SerialPort port)
        {
            try
            {
                port.Close();
                port.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
                port = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        //Execute AT Command
        public string ExecCommand(SerialPort port,string command, int responseTimeout, string errorMessage)
        {
            try
            {
               
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                receiveNow.Reset();
                port.Write(command + "\r");
           
                string input = ReadResponse(port, responseTimeout);
                if ((input.Length == 0) || ((!input.EndsWith("\r\n> ")) && (!input.EndsWith("\r\nOK\r\n"))))
                    throw new ApplicationException("No success message was received.");
                return input;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }   

        //Receive data from port
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ReadResponse(SerialPort port,int timeout)
        {
            string buffer = string.Empty;
            try
            {    
                do
                {
                    if (receiveNow.WaitOne(timeout, false))
                    {
                        string t = port.ReadExisting();
                        buffer += t;
                    }
                    else
                    {
                        if (buffer.Length > 0)
                            throw new ApplicationException("Response received is incomplete.");
                        else
                            throw new ApplicationException("No data received from phone.");
                    }
                }
                while (!buffer.EndsWith("\r\nOK\r\n") && !buffer.EndsWith("\r\n> ") && !buffer.EndsWith("\r\nERROR\r\n"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }

        #region Count SMS
        public int CountSMSmessages(SerialPort port)
        {
            int CountTotalMessages = 0;
            try
            {

                #region Execute Command

                string recievedData = ExecCommand(port, "AT", 300, "No phone connected at ");
                recievedData = ExecCommand(port, "AT+CMGF=0", 300, "Failed to set message format.");
                String command = "AT+CPMS?";
                recievedData = ExecCommand(port, command, 1000, "Failed to count SMS message");
                int uReceivedDataLength = recievedData.Length;

                #endregion

                #region If command is executed successfully
                if ((recievedData.Length >= 45) && ((recievedData.StartsWith("+CPMS")) || (recievedData.StartsWith("\r\n+CPMS"))))
                {

                    #region Parsing SMS
                    string[] strSplit = recievedData.Split(',');
                    string strMessageStorageArea1 = strSplit[0];     //SM
                    string strMessageExist1 = strSplit[1];           //Msgs exist in SM
                    #endregion

                    #region Count Total Number of SMS In SIM
                    CountTotalMessages = Convert.ToInt32(strMessageExist1);
                    #endregion

                }
                #endregion

                #region If command is not executed successfully
                else if (recievedData.Contains("ERROR"))
                {

                    #region Error in Counting total number of SMS
                    string recievedError = recievedData;
                    recievedError = recievedError.Trim();
                    recievedData = "Following error occured while counting the message" + recievedError;
                    #endregion

                }
                #endregion

                return CountTotalMessages;

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        #endregion

        #region Read SMS

        public AutoResetEvent receiveNow;

        public ShortMessageCollection ReadSMS(SerialPort port, string p_strCommand)
        {

            // Set up the phone and read the messages
            ShortMessageCollection messages = null;
            try
            {

                #region Execute Command
                // Check connection
                ExecCommand(port,"AT", 300, "No phone connected");
                // Use message format "Text mode"
                //ExecCommand(port,"AT+CMGF=1", 300, "Failed to set message format.");

                // Use message format "PDU mode"
                ExecCommand(port,"AT+CMGF=0", 300, "Failed to set message format.");
                // Use character set "PCCP437"
                // ExecCommand(port,"AT+CSCS=\"PCCP437\"", 300, "Failed to set character set.");
                // Use character set "Unicode"
                //ExecCommand(port,"AT+CSCS=\"UCS2\"", 300, "Failed to set character set.");

                // Select SIM storage
                ExecCommand(port,"AT+CPMS=\"SM\"", 300, "Failed to select message storage.");
                // Read the messages
                string input = ExecCommand(port, p_strCommand, 5000, "Failed to read the messages.");
                //string input = ExecCommand(port, "AT+CMGL=4", 5000, "Failed to read the messages.");
                //string input = ExecCommand(port, "AT+CMGR=1", 5000, "Failed to read the messages.");
                
                //string input = ExecCommand(port, "AT+CNMI=2,1,0,0,0\r", 300, "Failed to read the messages.");
               

                #endregion

                #region Parse messages
                messages = ParseMessages(input);
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (messages != null)
                return messages;
            else
                return null;
        
        }
        public ShortMessageCollection ParseMessages(string input)
        {
            ShortMessageCollection messages = new ShortMessageCollection();
            try
            {
                string[] tmp1 = input.Split(Convert.ToChar(10));
                for (int jjj = 0; jjj < tmp1.Length; jjj++)
                {
                    input = tmp1[jjj];
                    if (input.StartsWith("+CMGL:"))
                    {
                        string pduSource = tmp1[jjj + 1];
                        SMSType smsType = SMSBase.GetSMSType(pduSource);
                        switch (smsType)
                        {
                            case SMSType.SMS:
                                SMS sms = new SMS();
                                SMS.Fetch(sms, ref pduSource);
                                ShortMessage msg = new ShortMessage();
                                msg.Sender = sms.PhoneNumber;
                                msg.Message = sms.Message;
                                //msg.
                                messages.Add(msg);
                                //Use instance of SMS class here
                                break;
                            case SMSType.StatusReport:
                                SMSStatusReport statusReport = new SMSStatusReport();
                                SMSStatusReport.Fetch(statusReport, ref pduSource);
                                //Use instance of SMSStatusReport class here
                                break;
                        }
                    }
                }
                //string[] split = input.Split(Convert.ToChar(10));
                //foreach (string PDUsmsMessage in split)
               // {
                /*string pduSource = input;
                    SMSType smsType = SMSBase.GetSMSType(pduSource);
                    switch (smsType)
                    {
                        case SMSType.SMS:
                            SMS sms = new SMS();
                            SMS.Fetch(sms, ref pduSource);
                            ShortMessage msg = new ShortMessage();
                            msg.Message = sms.Message;
                            //Use instance of SMS class here
                            break;
                        case SMSType.StatusReport:
                            SMSStatusReport statusReport = new SMSStatusReport();
                            SMSStatusReport.Fetch(statusReport, ref pduSource);
                            //Use instance of SMSStatusReport class here
                            break;
                    }*/
                //}
                /*Regex r = new Regex(@"\+CMGL: (\d+),""(.+)"",""(.+)"",(.*),""(.+)""\r\n(.+)\r\n");
                Match m = r.Match(input);
                while (m.Success)
                {
                    ShortMessage msg = new ShortMessage();
                    //msg.Index = int.Parse(m.Groups[1].Value);
                    msg.Index = m.Groups[1].Value;
                    msg.Status = m.Groups[2].Value;
                    msg.Sender = m.Groups[3].Value;
                    msg.Alphabet = m.Groups[4].Value;
                    msg.Sent = m.Groups[5].Value;

                    PDUdecoder decode = new PDUdecoder();
                    string pduSource = m.Groups[6].Value;
                    //msg.Message = decode.GetPDU(m.Groups[6].Value);
                    SMSType smsType = SMSBase.GetSMSType(pduSource);
                    switch (smsType)
                    {
                        case SMSType.SMS:
                            SMS sms = new SMS();
                            SMS.Fetch(sms, ref pduSource);
                            msg.Message = sms.Message;
                            //Use instance of SMS class here
                            break;
                        case SMSType.StatusReport:
                            SMSStatusReport statusReport = new SMSStatusReport();
                            SMSStatusReport.Fetch(statusReport, ref pduSource);
                            //Use instance of SMSStatusReport class here
                            break;
                    }
                    



                    messages.Add(msg);

                    m = m.NextMatch();
                }

               /* string[] split = input.Split(Convert.ToChar(10));
                string mem = "";
                int index = 0;
                for (int i = 1; i < split.Length; i++)
                {
                    input = split[i];
                    if (split[i].StartsWith("+CMTI:"))
                    {
                        input = split[i] + ",";
                        input = input.Substring(6);
                        string[] tmp = input.Split(',');
                        mem = tmp[0];
                        mem = mem.ToLower();
                        index = Convert.ToInt32(tmp[1]);

                        index = Convert.ToInt32(tmp[1]);
                        sport.WriteLine("AT+CPMS=" + mem + "\r");
                        Thread.Sleep(300);
                        str = sport.ReadExisting();
                        sport.WriteLine("AT+CMGR=" + index.ToString() + "\r");
                        Thread.Sleep(500);
                        str = sport.ReadExisting();
                        string[] tmp1 = str.Split(Convert.ToChar(10));
                        for (int jjj = 0; jjj < tmp1.Length; jjj++)
                        {
                            str = tmp1[jjj];
                            if (str.StartsWith("+CMGR:"))
                            {
                                RcvdPDU = tmp1[jjj + 1];
                                PDUdecoder decode = new PDUdecoder();
                                msg = decode.GetPDU(RcvdPDU);
                                string mSa = decode.SMSCAddress;
                                string tOm = decode.SMSType;
                                string pid = decode.ProtocolID;
                                string sndrNo = decode.SenderMobileno;
                                string DcS = decode.DataCodingScheme;
                                string time = decode.TimeStamp;
                            }
                        }

                        //PDUdecoder decode = new PDUdecoder();
                        //str =decode.GetPDU(str);

                    }

                }*/

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return messages;
        }

        #endregion

        #region Send SMS
       
        static AutoResetEvent readNow = new AutoResetEvent(false);

        public bool sendMsg(SerialPort port, string PhoneNo, string Message, SMS.SMSEncoding encoding=SMS.SMSEncoding.UCS2)
        {
            bool isSend = false;

            try
            {

                SMS sms = new SMS();
                sms.Direction = SMSDirection.Submited;
                sms.PhoneNumber = PhoneNo;
                sms.ValidityPeriod = new TimeSpan(4, 0, 0, 0);
                sms.Message = Message;
                string pduSource = sms.Compose(encoding);



                
                
                //string command = "AT+CMGS=" + Convert.ToString(pduSource.Length / 2 - 1) + "\r" + pduSource + (char)26; //  "\u001a"
                //string command = "AT+CMGS=12" + "\r" + pduSource + "\u001a";



                string recievedData = ExecCommand(port, "AT", 300, "No phone connected");
                /*recievedData = ExecCommand(port,"AT+CMGF=1", 300, "Failed to set message format.");
                String command = "AT+CMGS=\"" + PhoneNo + "\"";
                recievedData = ExecCommand(port,command, 300, "Failed to accept phoneNo");         
                command = Message + char.ConvertFromUtf32(26) + "\r";*/
                recievedData = ExecCommand(port, "AT+CMGF=0", 300, "Failed to set message format.");


                //recievedData = ExecCommand(port, command, 3000, "Failed to send message"); //3 seconds
                string command = "AT+CMGS=" + (pduSource.Length / 2 - 1);
                port.NewLine = "\r\n";
                port.ReadTimeout = 10000;
                port.WriteLine(command);
                port.ReadTo("> ");
                //recievedData = ReadResponse(port, 3000);
                //port.ReadTo("> ");
                port.WriteLine(pduSource + (char)0x001A);
                port.ReadTo("+CMGS: ");
                recievedData = ReadResponse(port, 3000);
                

                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isSend = true;
                }
                else if (recievedData.Contains("ERROR"))
                {
                    isSend = false;
                }
                return isSend;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }   
        static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                    readNow.Set();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete SMS
        public bool DeleteMsg(SerialPort port , string p_strCommand)
        {
            bool isDeleted = false;
            try
            {

                #region Execute Command
                string recievedData = ExecCommand(port,"AT", 300, "No phone connected");
                recievedData = ExecCommand(port,"AT+CMGF=1", 300, "Failed to set message format.");
                String command = p_strCommand;
                recievedData = ExecCommand(port,command, 300, "Failed to delete message");
                #endregion

                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isDeleted = true;
                }
                if (recievedData.Contains("ERROR"))
                {
                    isDeleted = false;
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex; 
            }
            
        }  
        #endregion

    }
}
