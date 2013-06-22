using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SQLDataAccessLayer;
using System.Data.Common;
using System.Security.Cryptography;

namespace SQLDataAccessLayerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataProvider = ConfigurationManager.AppSettings["dataProvider"];
            string connectionString = ConfigurationManager.ConnectionStrings[dataProvider].ConnectionString;
            Console.WriteLine(dataProvider + "-----" + connectionString);
            
            SQLConnectionFactory sqlConnection = new SQLConnectionFactory(DbProviderFactories.GetFactory(dataProvider), connectionString);

            sqlConnection.Connection.Open();
            if (sqlConnection.Connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Connection to database is established");

                // Try to inserting and selecting data
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] originalBytes = ASCIIEncoding.Default.GetBytes("password");
                byte[] encodedBytes = md5.ComputeHash(originalBytes);

                //Convert encoded bytes back to a 'readable' string
                StringBuilder s = new System.Text.StringBuilder();
                foreach (byte b in encodedBytes)
                {
                    s.Append(b.ToString("x2").ToLower());
                }
                
                string password = s.ToString();
                
                
                SqlManager sqlManager = new SqlManager(sqlConnection.Connection);
                sqlManager.InsertInto(
                    "users",
                    new string[] { "id", "name", "username", "password", "privilege", "level" },
                    new object[] { 1, "indra", "tak3r", password, "admin", "1"},
                    string.Empty);

                object[] result = sqlManager.SelectFrom(
                    "users",
                    new string[] { "id", "name", "username", "password" },
                    string.Empty);

                foreach (object record in result)
                {
                    Dictionary<string, object> dRecord = record as Dictionary<string, object>;

                    if (dRecord != null)
                    {
                        foreach (KeyValuePair<string, object> pair in dRecord)
                        {
                            Console.WriteLine(string.Format("{0}:{1}", pair.Key, pair.Value.ToString()));
                        }
                    }
                }

                sqlManager.Update("users", new string[] { "name" }, new object[] { "kurniawan" }, "id=1");

                result = sqlManager.SelectFrom(
                    "users",
                    new string[] { "id", "name", "username", "password" },
                    string.Empty);

                foreach (object record in result)
                {
                    Dictionary<string, object> dRecord = record as Dictionary<string, object>;

                    if (dRecord != null)
                    {
                        foreach (KeyValuePair<string, object> pair in dRecord)
                        {
                            Console.WriteLine(string.Format("{0}:{1}", pair.Key, pair.Value.ToString()));
                        }
                    }
                }

                //int rowAffected = sqlManager.DeleteFrom("users", "id=1");

                //if (rowAffected == 1)
                //{
                //    Console.WriteLine("Successfully delete a record");
                //}
            }
            else
            {
                Console.WriteLine("Connection to database is not established");
            }
        }
    }
}
