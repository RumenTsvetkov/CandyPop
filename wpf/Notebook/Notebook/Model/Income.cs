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

        public string Note { get; set; }

        public string Invoice_number { get; set; }

        private DbAccess dbConnection;

        private SqlManager sqlManager;

        public Income(DbAccess dbAccess)
        {
            this.dbConnection = dbAccess;
            this.sqlManager = this.dbConnection.GetDBConnection();

            this.Items = new List<Product>();
        }        

        public void Load(string id)
        {
            // TODO: load the data from database.
            // function call to db to load stuff
            // LoadDataFromDb(this.Date)
            object[] result = sqlManager.SelectFrom(
                   "Income",
                   new string[] { "NB_FAKTUR", "DT_DATE", "NM_CLIENT", "DS_ADDRESS", "NM_ITEM", "QT_PRICE", "QT_QUANTITY" },
                   "NB_FAKTUR = " + id.ToString() );//string.Empty);

            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                Invoice_number = temp["NB_FAKTUR"].ToString();
                Date = (DateTime)temp["DT_DATE"];
                Buyer = temp["NM_CLIENT"].ToString();
                Address = temp["DS_ADDRESS"].ToString();

                Product item = new Product();
                item.Name = temp["NM_ITEM"].ToString();
                item.Price = (float)temp["QT_PRICE"];
                item.Quantity = (int)temp["QT_QUANTITY"];
                Items.Add(item );
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
                        "Income",
                        new string[] { "NB_FAKTUR", "DT_DATE","NM_CLIENT", "DS_ADDRESS", "NM_ITEM","QT_PRICE", "QT_QUANTITY" },
                        new object[] { Invoice_number, Date, Buyer, Address, item.Name, item.Price, item.Quantity},
                        string.Empty);
            }
        }


    }
}
