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

        private Expense expense;

        public ExpenseForm(DbAccess dbAccess)
        {
            InitializeComponent();
            this.dbAccess = dbAccess;

            this.expense = new Expense(this.dbAccess);
            this.dgProducts.ItemsSource = this.Products;
        }

        public ExpenseForm(DbAccess dbAccess, Expense expense)
        {
            InitializeComponent();
            this.dbAccess = dbAccess;
            this.expense = expense;

            // populate the form with the given expense data.
            this.tbInvoiceNo.Text = this.expense.InvoiceNumber;
            this.tbSeller.Text = this.expense.Seller;
            this.tbAddress.Text = this.expense.Address;
            this.datePicker.SelectedDate = this.expense.Date;

            foreach (var product in this.expense.Items)
            {
                this.Products.Add(product);
            }

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

            this.expense.InvoiceNumber = this.tbInvoiceNo.Text;
            this.expense.Seller = this.tbSeller.Text;
            this.expense.Address = this.tbAddress.Text;
            this.expense.Date = (DateTime)this.datePicker.SelectedDate;

            foreach (var product in this.products)
            {
                this.expense.Items.Add(product);
            }

            this.expense.Save();

            this.DialogBox("Transaksi tersimpan.", "Success");
            this.Close();
        }

        private MessageBoxResult DialogBox(string message, string caption)
        {
            return MessageBox.Show(message, caption);
        }
    }
}
