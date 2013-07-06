
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
            // validation
            var error = string.Empty;

            if (string.IsNullOrEmpty(this.tbInvoiceNo.Text))
            {
                error += "Error: Masukan no faktur penjualan !!\n";
            }

            if (string.IsNullOrEmpty(this.tbBuyer.Text))
            {
                error += "Error: Masukan nama pembeli !!\n";
            }

            if (string.IsNullOrEmpty(this.tbAddress.Text))
            {
                error += "Error: Masukan alamat pembeli !!\n";
            }

            if (this.datePicker.SelectedDate == null)
            {
                error += "Error: Masukan tanggal faktur penjualan !!\n";
            }

            if (this.products.Count == 0)
            {
                error += "Error: Masukan minimal 1 produk dalam transaksi !!\n";
            }

            if (!string.IsNullOrEmpty(error))
            {
                this.DialogBox(error, "Error");
                return;
            }

            var income = new Income(this.dbAccess);

            income.InvoiceNumber = this.tbInvoiceNo.Text;
            income.Buyer = this.tbBuyer.Text;
            income.Address = this.tbAddress.Text;
            income.Date = (DateTime)this.datePicker.SelectedDate;

            foreach (var product in this.products)
            {
                income.Items.Add(product);
            }

            income.Save();

            this.DialogBox("Transaksi tersimpan.", "Success");
            this.Close();
        }

        private MessageBoxResult DialogBox(string message, string caption)
        {
            return MessageBox.Show(message, caption);
        }
    }
}
