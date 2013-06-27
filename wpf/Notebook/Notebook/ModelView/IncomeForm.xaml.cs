
namespace Notebook.ModelView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Notebook.Model;
using System.Collections.ObjectModel;

    /// <summary>
    /// Interaction logic for IncomeForm.xaml
    /// </summary>
    public partial class IncomeForm : Window
    {
        private DbAccess dbAccess;

        private ObservableCollection<Product> products = new ObservableCollection<Product>();

        // TODO: check the form, print out error if an incorrect value is given.

        public IncomeForm(DbAccess dbAccess)
        {
            InitializeComponent();
            this.dbAccess = dbAccess;

            this.dgProducts.ItemsSource = this.Products;
        }

        internal ObservableCollection<Product> Products
        {
            get
            {
                return this.products;
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            var income = new Income(this.dbAccess);

            income.Invoice_number = this.tbInvoiceNo.Text;
            income.Buyer = this.tbBuyer.Text;
            income.Address = this.tbAddress.Text;
            income.Date = (DateTime)this.datePicker.SelectedDate;

            foreach (var product in this.products)
            {
                income.Items.Add(product);
            }

            income.Save();
        }
    }
}
