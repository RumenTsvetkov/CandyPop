namespace Notebook
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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
    using Notebook.Reports;
    using SQLDataAccessLayer;

    // TODO:
    // 1. feature for generating table report (Indra).
    // 2. feature for generating graph report (Indra).
    // 2. feature for displaying sum (Indra).
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DbAccess dbAccess;

        private ObservableCollection<Transactions> transactions = new ObservableCollection<Transactions>();

        private float creditBalance;

        private float debitBalance;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            this.dbAccess = new DbAccess();           
            this.table.ItemsSource = this.transactions;
        }

        public string CreditBalance
        {
            get
            {
                return string.Format(Messages.CurrencyFormatting, this.creditBalance, "CR");
            }
        }

        public string DebitBalance
        {
            get
            {
                return string.Format(Messages.CurrencyFormatting, this.debitBalance, "DB");
            }
        }

        public string Balance
        {
            get
            {
                var balance = this.creditBalance - this.debitBalance;
                return string.Format(
                    Messages.CurrencyFormatting, 
                    balance,
                    balance < 0 ? "DB" : "CR");
            }
        }

        private bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            this.OnPropertyChange(propertyName);            
            return true;
        }

        private void OnPropertyChange(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void IncomeButtonClicked(object sender, RoutedEventArgs e)
        {
            var incomeForm = new IncomeForm(this.dbAccess);
            incomeForm.Show();
        }

        private void ExpenseButtonClicked(object sender, RoutedEventArgs e)
        {
            var expenseForm = new ExpenseForm(this.dbAccess);
            expenseForm.Show();
        }

        private void FindTransactions(DateTime startDate, DateTime endDate)
        {
            var result = this.GetTransactionsByDates(startDate, endDate, "Income");
            
            var transactions = new List<string>();
            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                transactions.Add(temp["NB_FAKTUR"].ToString());
            }

            transactions = transactions.Distinct().ToList();
            foreach (string trans in transactions)
            {
                var income = new Income(this.dbAccess);
                income.Load(trans);
                this.AddTransaction(income);
            }

            result = this.GetTransactionsByDates(startDate, endDate, "Expense");
            transactions = new List<string>();
            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                transactions.Add(temp["NB_FAKTUR"].ToString());
            }

            transactions = transactions.Distinct().ToList();
            foreach (string trans in transactions)
            {
                var expense = new Expense(this.dbAccess);
                expense.Load(trans);
                this.AddTransaction(expense);
            }
        }

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
                creditBalance = 0;
                debitBalance = 0;
                this.transactions.Clear();
                this.FindTransactions((DateTime)dStart.SelectedDate, (DateTime)dEnd.SelectedDate);
                this.transactions = new ObservableCollection<Transactions>(from i in this.transactions orderby i.Date select i);
                this.table.ItemsSource = this.transactions;
                
                if (this.transactions.Count == 0)
                {
                    MessageBox.Show("Tidak ditemukan daftar transaksi");
                }
            }
        }

        private void ViewTransactionClicked(object sender, RoutedEventArgs e)
        {
            var transaction = this.transactions.Where(t => t.InvoiceNumber == (sender as Button).Tag.ToString()).First<Transactions>();
            if (transaction is Income)
            {
                var incomeReport = new IncomeReport((Income)transaction);
                incomeReport.Show();
            }
            else
            {
                var expenseReport = new ExpenseReport((Expense)transaction);
                expenseReport.Show();
            }
        }

        private void sheet_Click(object sender, RoutedEventArgs e)
        {
        }

        private object[] GetTransactionsByDates(DateTime startDate, DateTime endDate, string dbtablename)
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

        private void EditTransactionClicked(object sender, RoutedEventArgs e)
        {
            var transaction = this.transactions.Where(t => t.InvoiceNumber == (sender as Button).Tag.ToString()).First<Transactions>();

            if (transaction != null)
            {
                if (transaction is Income)
                {
                    var incomeForm = new IncomeForm(this.dbAccess, (Income)transaction);
                    incomeForm.Show();
                }
                else
                {
                    var expenseForm = new ExpenseForm(this.dbAccess, (Expense)transaction);
                    expenseForm.Show();
                }
            }
        }

        private void DeleteTransactionClicked(object sender, RoutedEventArgs e)
        {
            var transaction = this.transactions.Where(t => t.InvoiceNumber == (sender as Button).Tag.ToString()).First<Transactions>();
            var sqlManager = this.dbAccess.GetDBConnection();

            if (transaction != null)
            {
                if (transaction is Income)
                {
                    sqlManager.DeleteFrom("Income", string.Format("NB_FAKTUR = '{0}'", transaction.InvoiceNumber));
                }
                else
                {
                    sqlManager.DeleteFrom("Expense", string.Format("NB_FAKTUR = '{0}'", transaction.InvoiceNumber));
                }

                //update transaction view
                this.RemoveTransaction(transaction);
                this.table.ItemsSource = this.transactions;
            }
        }

        private void AddTransaction(Transactions transaction)
        {
            this.transactions.Add(transaction);

            // update the balance
            if (transaction is Income)
            {
                this.SetField<float>(ref this.creditBalance, this.creditBalance + transaction.Total, "CreditBalance");
            }
            else
            {
                this.SetField<float>(ref this.debitBalance, this.debitBalance + transaction.Total, "DebitBalance");
            }

            this.OnPropertyChange("Balance");
        }

        private void RemoveTransaction(Transactions transaction)
        {
            this.transactions.Remove(transaction);

            // update the balance
            if (transaction is Income)
            {
                this.SetField<float>(ref this.creditBalance, this.creditBalance - transaction.Total, "CreditBalance");
            }
            else
            {
                this.SetField<float>(ref this.debitBalance, this.debitBalance - transaction.Total, "DebitBalance");
            }

            this.OnPropertyChange("Balance");
        }
    }
}
