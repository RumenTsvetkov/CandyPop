
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

    /// <summary>
    /// Interaction logic for IncomeForm.xaml
    /// </summary>
    public partial class IncomeForm : Window
    {
        private DbAccess dbAccess;

        // TODO: populate the products.
        // TODO: check the form, print out error if an incorrect value is given.

        public IncomeForm(DbAccess dbAccess)
        {
            InitializeComponent();

            this.dbAccess = dbAccess;
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            var income = new Income(this.dbAccess);

            income.Invoice_number = this.tbInvoiceNo.Text;
            income.Buyer = this.tbBuyer.Text;
            income.Address = this.tbAddress.Text;
            income.Date = (DateTime)this.datePicker.SelectedDate;

            income.Save();
        }
    }
}
