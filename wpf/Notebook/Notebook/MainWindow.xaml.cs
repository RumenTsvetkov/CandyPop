namespace Notebook
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Notebook.Model;
    using Notebook.ModelView;
    using SQLDataAccessLayer;
    using Notebook.Reports;

    // TODO:
    // 1. make the transactions editable (to be discussed).
    // 2. make the transcations deletable (to be discussed).
    // 3. feature for generating graph report (Indra).
    // 4. Added expense table into database, have a look at Expense.cs to find out what need to be stored in the database (Felix). 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbAccess dbAccess;

        private ObservableCollection<Transactions> transactions = new ObservableCollection<Transactions>();

        public MainWindow()
        {
            InitializeComponent();

            this.dbAccess = new DbAccess();           
            this.table.ItemsSource = this.transactions;
        }

        private void IncomeButtonClicked(object sender, RoutedEventArgs e)
        {
            var incomeForm = new IncomeForm(this.dbAccess);
            incomeForm.Show();
        }

        // TODO: should search both incomes and expenditure
        private void FindIncomeTransactions(DateTime startDate, DateTime endDate)
        {
            var result = this.getFaktursByDates(startDate, endDate, "Income");
            
            var fakturs = new List<string>();
            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                fakturs.Add(temp["NB_FAKTUR"].ToString());
            }

            var faktursNoDupes = fakturs.Distinct().ToList();
            foreach (string faktur in faktursNoDupes)
            {
                var income = new Income(this.dbAccess);
                income.Load(faktur);
                this.transactions.Add(income);
            }            
        }
        private void FindExpenseTransactions(DateTime startDate, DateTime endDate)
        {
            var result = this.getFaktursByDates(startDate, endDate, "Expense"); 

            var fakturs = new List<string>();
            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                fakturs.Add(temp["NB_FAKTUR"].ToString());
            }

            var faktursNoDupes = fakturs.Distinct().ToList();
            foreach (string faktur in faktursNoDupes)
            {
                var expense = new Expense(this.dbAccess);
                expense.Load(faktur);
                this.transactions.Add(expense);
            }
        }
        // TODO: should display both incomes and expenditure (Gita)
        private void FindClicked(object sender, RoutedEventArgs e)
        {
            if (dStart.ToString() == "")
            {
                MessageBox.Show("Silahkan pilih tanggal mulai");
            }
            else if (dEnd.ToString() == "")
            {
                MessageBox.Show("Silahkan pilih tanggal akhir");

            }
            else if (dEnd.SelectedDate < dStart.SelectedDate)
            {
                MessageBox.Show("Silahkan pilih tanggal awal lebih kecil dr tanggal akhir");
            }
            else
            {
                this.transactions.Clear();
                this.FindIncomeTransactions((DateTime)dStart.SelectedDate, (DateTime)dEnd.SelectedDate);
                this.FindExpenseTransactions((DateTime)dStart.SelectedDate, (DateTime)dEnd.SelectedDate);
                if (this.transactions.Count == 0)
                {
                    MessageBox.Show("Tidak ditemukan daftar transaksi");
                }
            }
        }

        private void ExpenseButtonClicked(object sender, RoutedEventArgs e)
        {
            var expenseForm = new ExpenseForm(this.dbAccess);
            expenseForm.Show();
        }

        // TODO: make it work for both income and expenditure. (Gita)
        private void ViewTransactionClicked(object sender, RoutedEventArgs e)
        { 
            for (var i = 0; i < this.transactions.Count; i++)
                if (this.transactions[i].Debit == "-")
                {
                    if (this.transactions[i].InvoiceNumber == (sender as Button).Tag.ToString())
                    {
                        var incomeReport = new IncomeReport((Income)this.transactions[i]);
                        incomeReport.Show();
                    }
                }
                else
                {
                    if (this.transactions[i].InvoiceNumber == (sender as Button).Tag.ToString())
                    {
                        var expenseReport = new ExpenseReport((Expense)this.transactions[i]);
                        expenseReport.Show();
                       
                    }
                }

            
            /*
             * Income selectedIncome = this.transactions.Where(
                i => (sender as Button).Tag.ToString() == (i as Income).InvoiceNumber).Single() as Income;
            var incomeReport = new IncomeReport(selectedIncome);
            incomeReport.Show();
             */
        }

        private void sheet_Click(object sender, RoutedEventArgs e)
        {
        }

        private object[] getFaktursByDates(DateTime startDate, DateTime endDate, string dbtablename)
        {
            var sqlManager = this.dbAccess.GetDBConnection();

            var newStartDate = startDate.ToString("yyyy-MM-dd");
            var newEndDate = endDate.ToString("yyyy-MM-dd");

            object[] result = sqlManager.SelectFrom(
                   dbtablename,
                   new string[] { "NB_FAKTUR" },
                   "DT_DATE between " + "'" + newStartDate + "' and '" + newEndDate + "'");
            return result;
        }
    }
}
