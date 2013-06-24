namespace Notebook.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Income
    {
        public DateTime Date { get; set; }

        public string Buyer { get; set; }

        public string Address { get; set; }

        public string NPWP { get; set; }

        public List<Product> Items { get; private set; }

        string Note { get; set; }

        public string Invoice_number { get; set; }

        public Income(string invoice_number, string buyer, string address, DateTime date)
        {
            this.Invoice_number = invoice_number;
            this.Date = date;
            this.Buyer = buyer;
            this.Address = address;
            this.Items = new List<Product>();
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
            this.Items.Add(item);
        }

        public void Load()
        {
            // TODO: load the data from database.
            // function call to db to load stuff
            // LoadDataFromDb(this.Date)
        }

        public void Save()
        {
            // TODO: Store the data into database.
            foreach (Product item in this.Items)
            {
                // function call to db to store each item
                // SaveDataToDb(this.Invoice_number, this.Date, this.Buyer, this.Address, item.Name, item.Price, item.Quantity)

            }
        }
    }
}
