namespace Notebook.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Expense : Transactions
    {
        public string Seller { get; set; }

        public Expense(DbAccess dbAccess)
        {
            this.dbConnection = dbAccess;
            this.sqlManager = this.dbConnection.GetDBConnection();

            this.Items = new List<Product>();
        }

        public override void Load(string id)
        {
            object[] result = sqlManager.SelectFrom(
                   "Income",
                   new string[] { "NB_FAKTUR", "DT_DATE", "NM_CLIENT", "DS_ADDRESS", "NM_ITEM", "QT_PRICE", "QT_QUANTITY" },
                   string.Format("NB_FAKTUR = '{0}'", id));

            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                InvoiceNumber = temp["NB_FAKTUR"].ToString();
                Date = (DateTime)temp["DT_DATE"];
                Seller = temp["NM_CLIENT"].ToString();
                Address = temp["DS_ADDRESS"].ToString();

                Product item = new Product();
                item.Name = temp["NM_ITEM"].ToString();
                item.Price = (float)Convert.ChangeType(temp["QT_PRICE"], typeof(float));
                item.Quantity = (int)Convert.ChangeType(temp["QT_QUANTITY"], typeof(int));
                Items.Add(item);
            }
        }

        public override void Save()
        {
            // Store the data into database.
            foreach (Product item in Items)
            {
                // function call to db to store each item
                // SaveDataToDb(this.Invoice_number, this.Date, this.Buyer, this.Address, item.Name, item.Price, item.Quantity)
                sqlManager.InsertInto(
                        "Expense",
                        new string[] { "NB_FAKTUR", "DT_DATE", "NM_CLIENT", "DS_ADDRESS", "NM_ITEM", "QT_PRICE", "QT_QUANTITY" },
                        new object[] { InvoiceNumber, Date, Seller, Address, item.Name, item.Price, item.Quantity },
                        string.Empty);
            }
        }
    }
}
