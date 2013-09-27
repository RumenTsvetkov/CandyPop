

namespace Notebook.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SQLDataAccessLayer;
    
    public abstract class Transactions
    {
        public DateTime Date { get; set; }

        public string InvoiceNumber { get; set; }

        public string Address { get; set; }

        public string NPWP { get; set; }

        public string Note { get; set; }

        public List<Product> Items { get; protected set; }

        protected DbAccess dbConnection;

        protected SqlManager sqlManager;

        public float Total 
        { 
            get
            {
                return this.CalculateGrandTotal();
            }
        }

        public string Credit
        {
            get
            {
                if (this is Income)
                {
                    return string.Format(Messages.CurrencyFormatting2, this.Total);
                }

                return "-";
            }
        }

        public string Debit
        {
            get
            {
                if (this is Expense)
                {
                    return string.Format(Messages.CurrencyFormatting2, this.Total);
                }

                return "-";
            }
        }

        public abstract void Load(string id);

        public abstract void Save();

        private float CalculateGrandTotal()
        {
            var total = 0.0f;

            foreach (var product in this.Items)
            {
                total += product.Price * product.Quantity;
            }

            return total;
        }
    }
}
