namespace Notebook.ModelView
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

    /// <summary>
    /// Interaction logic for ExpenseForm.xaml
    /// </summary>
    public partial class ExpenseForm : Window
    {
        private DbAccess dbAccess;

        private ObservableCollection<Product> products = new ObservableCollection<Product>();

        public ExpenseForm(DbAccess dbAccess)
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
                error += "Error: Masukan no faktur pembelian !!\n";
            }

            if (string.IsNullOrEmpty(this.tbSeller.Text))
            {
                error += "Error: Masukan nama penjual !!\n";
            }

            if (string.IsNullOrEmpty(this.tbAddress.Text))
            {
                error += "Error: Masukan alamat penjual !!\n";
            }

            if (this.datePicker.SelectedDate == null)
            {
                error += "Error: Masukan tanggal faktur pembelian !!\n";
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

            var expense = new Expense(this.dbAccess);

            expense.InvoiceNumber = this.tbInvoiceNo.Text;
            expense.Seller = this.tbSeller.Text;
            expense.Address = this.tbAddress.Text;
            expense.Date = (DateTime)this.datePicker.SelectedDate;

            foreach (var product in this.products)
            {
                expense.Items.Add(product);
            }

            expense.Save();

            this.DialogBox("Transaksi tersimpan.", "Success");
            this.Close();
        }

        private MessageBoxResult DialogBox(string message, string caption)
        {
            return MessageBox.Show(message, caption);
        }
    }
}
