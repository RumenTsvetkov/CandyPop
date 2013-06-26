using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SQLDataAccessLayer;
using System.Data.Common;

namespace Notebook
{
    public class DbAccess
    {

        public SqlManager GetDBConnection()
        {
            string dataProvider = ConfigurationManager.AppSettings["dataProvider"];
            string connectionString = ConfigurationManager.ConnectionStrings[dataProvider].ConnectionString;
            SQLConnectionFactory sqlConnection = new SQLConnectionFactory(DbProviderFactories.GetFactory(dataProvider), connectionString);
            sqlConnection.Connection.Open();
            SqlManager sqlManager = new SqlManager(sqlConnection.Connection);
            
            if (sqlConnection.Connection.State == System.Data.ConnectionState.Open)
                return sqlManager;

            return null;
        }
    }
}
