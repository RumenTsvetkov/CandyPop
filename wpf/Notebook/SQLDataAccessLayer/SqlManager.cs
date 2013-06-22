using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SQLDataAccessLayer
{
    public class SqlManager : ISqlQuery
    {
        private IDbConnection connection;

        public SqlManager(IDbConnection connection)
        {
            this.connection = connection;
        }

        #region ISqlQuery Members

        public int InsertInto(string tableName, string[] columnsName, object[] values, string condition)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name can not be null or empty");
            }

            if (columnsName.Length != values.Length)
            {
                throw new ArgumentException("Column length must be the same with values length");
            }

            string query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, this.ConstructParameters(columnsName, string.Empty), this.ConstructParameters(columnsName, ":"));

            if (!string.IsNullOrEmpty(condition))
            {
                query += string.Format(" WHERE {0}", condition);
            }

            Console.WriteLine(query);
            
            IDbCommand command = this.GetCommand(query);

            for (int i = 0; i < columnsName.Length; i++)
            {
                IDataParameter param = command.CreateParameter();
                param.ParameterName = columnsName[i];
                param.Value = values[i];

                command.Parameters.Add(param) ;
            }

            int rowAffected = command.ExecuteNonQuery();

            return rowAffected;
        }

        public int Update(string tableName, string[] columnsName, object[] values, string condition)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name can not be null or empty");
            }

            if (columnsName.Length != values.Length)
            {
                throw new ArgumentException("Column length must be the same with values length");
            }

            string query = string.Format("UPDATE {0} SET ", tableName);

            
            for (int i = 0; i < columnsName.Length; i++)
            {
                if(i>0)
                {
                    query += ",";
                }

                query += columnsName[i] + "= :" + columnsName[i];
            }

            if (!string.IsNullOrEmpty(condition))
            {
                query += string.Format(" WHERE {0}", condition);
            }

            IDbCommand command = this.GetCommand(query);

            for (int i = 0; i < columnsName.Length; i++)
            {
                IDataParameter param = command.CreateParameter();
                param.ParameterName = columnsName[i];
                param.Value = values[i];

                command.Parameters.Add(param);
            }

            int rowAffected = command.ExecuteNonQuery();

            return rowAffected;
        }

        public object[] SelectFrom(string tableName, string[] columnsName, string condition)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name can not be null or empty");
            }

            string query = string.Format("SELECT {0} FROM {1}", this.ConstructParameters(columnsName), tableName);

            if (!string.IsNullOrEmpty(condition))
            {
                // search for custom string condition
                if (condition.Contains("WHERE") || condition.Contains("INNER JOIN"))
                {
                    query += condition;
                }
                else
                {
                    query += string.Format(" WHERE {0}", condition);
                }
            }

            IDbCommand command = this.GetCommand(query);

            List<object> result = new List<object>();

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Dictionary<string, object> record = new Dictionary<string, object>();

                    foreach (string c in columnsName)
                    {
                        record.Add(c, reader.GetValue(reader.GetOrdinal(c)));
                    }

                    result.Add(record);
                }
            }

            return result.ToArray();
        }

        public int DeleteFrom(string tableName, string condition)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name can not be null or empty");
            }

            string query = string.Format("DELETE FROM {0}", tableName);         

            if (!string.IsNullOrEmpty(condition))
            {
                query += string.Format(" WHERE {0}", condition);
            }

            IDbCommand command = this.GetCommand(query);

            int rowAffected = command.ExecuteNonQuery();

            return rowAffected;
        }

        #endregion

        private IDbCommand GetCommand(string query)
        {
            if(this.connection.State != ConnectionState.Open)
            {
                this.connection.Open();
            }

            IDbCommand command = this.connection.CreateCommand();
            command.CommandText = query;
            command.Connection = this.connection;

            return command;
        }

        private string ConstructParameters(string[] parameters)
        {
            return this.ConstructParameters(parameters, string.Empty);
        }

        private string ConstructParameters(string[] parameters, string ch)
        {
            string result = string.Empty;

            for(int i=0;i<parameters.Length;i++)
            {
                if (i > 0)
                {
                    result += ",";
                }

                result += ch + parameters[i];
            }

            return result;
        }

        private string ConstructValue(object[] values)
        {
            string result = string.Empty;

            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    result += ",";
                }

                result += "'" + values[i].ToString() + "'";
            }

            return result;
        }
    }
}
