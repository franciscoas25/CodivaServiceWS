using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Connection
{
    public static class CodivaServiceConnection
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["CodivaServiceConnectioString"].ConnectionString;

        public static IDbConnection GetConnection()
        {
            var conn = new OracleConnection(_connectionString);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            return conn;
        }

        public static void CloseConnection(IDbConnection conn)
        {
            if (conn.State == ConnectionState.Open || conn.State == ConnectionState.Broken)
            {
                conn.Close();
            }
        }
    }
}
