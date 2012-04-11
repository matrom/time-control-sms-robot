using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading;

namespace TimeControlServer
{
    class DatabaseManager :IDisposable
    {
        TimeControlServerDataSetTableAdapters.LogMessageAdapter logAdapter = new TimeControlServerDataSetTableAdapters.LogMessageAdapter();
        List<Message> Outbox;
        private bool StopChildThreads = false;
        string connectionString;
        private Thread CheckUsersStatusThread;
        public DatabaseManager(List<Message> Outbox)
        {
            this.Outbox = Outbox;
            connectionString = getConnectionString();
            CheckUsersStatusThread = new Thread(CheckUsersStatusRunner);
            CheckUsersStatusThread.Start();
        }
        private string getConnectionString()
        {
            string _connectionString = "";
            foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
            {
                if (connection.Name == "TimeControlServer.Properties.Settings.TimeControlServerConnectionString")
                    _connectionString = connection.ConnectionString;
            }
            return _connectionString;
        }
        public void LogMessage(Message mes)
        {
            int result = logAdapter.LogMessage(mes.id, mes.From, mes.To, mes.text, mes.isProcessed, mes.timeStamp);
            if (result == 0)
            {
                // handle error
            }
            
        }

        public void CheckUsersStatusRunner()
        {
            while (!StopChildThreads)
            {
                CheckUsersStatus();
                Thread.Sleep(180000);
            }
        }

        public void CheckUsersStatus()
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConnection1 = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            //SqlDataReader reader;

            cmd.CommandText = "GetUsersRunningOutOfTime";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            DataTableReader datatableReader = dt.CreateDataReader();
            bool newMessages = false;
            while (datatableReader.Read())
            {
                string FirstName = datatableReader["FirstName"].ToString();
                string Surname = datatableReader["Surname"].ToString();
                string PhoneNumber = datatableReader["PhoneNumber"].ToString();
                string warningType = datatableReader["warningType"].ToString();
                int minutesLeft =  Convert.ToInt32(datatableReader["minutesLeft"].ToString());
                string warningMessage;
                if ((minutesLeft <10 || minutesLeft >20) && (minutesLeft%10 == 1))
                    warningMessage = "Внимание: на Вашем счету осталась " + minutesLeft.ToString() + " минута!";
                else
                    if ((minutesLeft < 10 || minutesLeft > 20) && ((minutesLeft % 10 == 2) || (minutesLeft % 10 == 3) || (minutesLeft % 10 == 4)))
                        warningMessage = "Внимание: на Вашем счету остались " + minutesLeft.ToString() + " минуты!";
                    else
                        warningMessage = "Внимание: на Вашем счету осталось " + minutesLeft.ToString() + " минут!";

                Message mes = new Message();
                mes.To = PhoneNumber;
                switch (warningType)
                {
                    case "hour":
                    case "5minutes":
                        mes.text = warningMessage;
                        break;
                    case "OutOfTime":
                        mes.text = "Ваше время вышло";
                        break;
                }
                lock (Outbox)
                    Outbox.Add(mes);
                newMessages = true;

            }
            if (newMessages)
                ThreadManager.newMessageInOutbox.Set();
            
        }
        public void Dispose()
        {
            StopChildThreads = true;
        }
    }
}
