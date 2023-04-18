using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace ConsoleApp52
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "127.0.0.1";
            int port = 3306;
            string database = "build";
            string username = "root";
            string password = "Drakon2023";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
