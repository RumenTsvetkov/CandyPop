namespace Notebook.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SQLDataAccessLayer;
    using System.Configuration;
    using System.Data.Common;

    internal class Income
    {
        public DateTime Date { get; set; }

        public string Buyer { get; set; }

        public string Address { get; set; }

        public string NPWP { get; set; }

        public List<Product> Items { get; private set; }

        string Note { get; set; }

        public string Invoice_number { get; set; }
        private SqlManager sqlManager { get; set; }

        public Income(string invoice_number, string buyer, string address, DateTime date)
        {
            this.Invoice_number = invoice_number;
            this.Date = date;
            this.Buyer = buyer;
            this.Address = address;
            this.Items = new List<Product>();
            sqlManager = GetDBConnection();
        }

        public Income(DateTime date)
        {
            this.Date = date;
        }

        public void StoreAnItem(string name, float price, int quantity)
        {
            Product item = new Product();
            item.Name  = name;
            item.Price = price;
            item.Quantity = quantity;
            Items.Add(item);
        }

        public void Load()
        {
            // TODO: load the data from database.
            // function call to db to load stuff
            // LoadDataFromDb(this.Date)
            object[] result = sqlManager.SelectFrom(
                   "income",
                   new string[] { "invoice_number", "date", "buyer", "address", "item", "price", "quantity" },
                   string.Empty);

            foreach (object record in result)
            {

              
            }
        }

        public void Save()
        {
            // Store the data into database.
            foreach (Product item in Items)
            {
                // function call to db to store each item
                // SaveDataToDb(this.Invoice_number, this.Date, this.Buyer, this.Address, item.Name, item.Price, item.Quantity)
                sqlManager.InsertInto(
                        "income",
                        new string[] { "invoice_number", "date","buyer", "address", "item","price", "quantity" },
                        new object[] { Invoice_number, Date, Buyer, Address, item.Name, item.Price, item.Quantity},
                        string.Empty);
            }
        }

        public SqlManager GetDBConnection()
        {
            string dataProvider = ConfigurationManager.AppSettings["dataProvider"];
            string connectionString = ConfigurationManager.ConnectionStrings[dataProvider].ConnectionString;
            SQLConnectionFactory sqlConnection = new SQLConnectionFactory(DbProviderFactories.GetFactory(dataProvider), connectionString);
            sqlConnection.Connection.Open();
            SqlManager sqlManager = new SqlManager(sqlConnection.Connection);
            //if (sqlConnection.Connection.State == System.Data.ConnectionState.Open)
            return sqlManager;
            
           

        }

    }
}
