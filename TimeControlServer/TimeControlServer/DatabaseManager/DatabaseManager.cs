using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace TimeControlServer
{
    class DatabaseManager
    {
        TimeControlServerDataSetTableAdapters.LogMessageAdapter logAdapter = new TimeControlServerDataSetTableAdapters.LogMessageAdapter();
        public void LogMessage(Message mes)
        {
            int result =  logAdapter.LogMessage(mes.id, mes.From, mes.To, mes.text, mes.isProcessed, mes.timeStamp);
            if (result == 0)
            {
                // handle error
            }



            //TimeControlServerDataSet dataset = new TimeControlServerDataSet();
            
            /*using (SqlConnection connection = new SqlConnection("Data Source=(local);Initial Catalog=TimeControlServer;Integrated Security=True"))
            {
                connection.Open();
                Console.WriteLine("ServerVersion: {0}", connection.ServerVersion);
                Console.WriteLine("State: {0}", connection.State);
            }*/
        }
    }
}
