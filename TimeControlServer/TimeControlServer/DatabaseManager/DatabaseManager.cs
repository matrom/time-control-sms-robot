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

        public void ProcessMessage(Message mes, string folder)
        {
            int result = logAdapter.ProcessMessage(mes.id, mes.From, mes.To, mes.text, mes.isProcessed, mes.timeStamp, folder);
            if (result == 0)
            {
                // handle error
            }
            GetOutboxFromDb();
        }

        public void GetOutboxFromDb()
        {
             DataTable dt = new DataTable();
            SqlConnection sqlConnection1 = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            //SqlDataReader reader;

            cmd.CommandText = "GetOutbox";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            DataTableReader datatableReader = dt.CreateDataReader();
            while (datatableReader.Read())
            {
                Message mes = new Message();
                mes.id = new Guid(datatableReader["id"].ToString());
                mes.From = datatableReader["from"].ToString();
                mes.To = datatableReader["to"].ToString();
                mes.text = datatableReader["text"].ToString();
                mes.timeStamp = Convert.ToDateTime(datatableReader["ts"].ToString());
                mes.isProcessed = false;
                lock (Outbox)
                {
                    bool alreadyExists = false;
                    foreach (Message outboxMessage in Outbox)
                        if (outboxMessage.id == mes.id)
                            alreadyExists = true;
                    if (!alreadyExists)
                        Outbox.Add(mes);
                }
            }
        }

        public void CheckUsersStatusRunner()
        {
            while (!StopChildThreads)
            {
                GetOutboxFromDb();
                Thread.Sleep(180000);
            }
        }

        
        public void Dispose()
        {
            StopChildThreads = true;
        }
    }
}
