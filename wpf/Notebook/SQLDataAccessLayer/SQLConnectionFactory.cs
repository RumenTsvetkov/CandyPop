using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace SQLDataAccessLayer
{
    public class SQLConnectionFactory
    {
        private IDbConnection connection;

        public SQLConnectionFactory(DbProviderFactory factory, string connectionString)
        {
            this.connection = factory.CreateConnection();
            this.connection.ConnectionString = connectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

    }
}
