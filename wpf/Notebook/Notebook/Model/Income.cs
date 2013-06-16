namespace Notebook.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Income
    {
        public DateTime Date {get;set;}

        public string Buyer { get; set; }

        public string Address { get; set; }

        public string NPWP { get; set; }

        public List<Product> Items { get; private set; }

        string Note { get; set; }

        public Income()
        {
            this.Items = new List<Product>();
        }
    }
}
