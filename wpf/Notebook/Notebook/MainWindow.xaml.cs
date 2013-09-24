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
                this.transactions.Add(income);
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
                this.transactions.Add(expense);
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

        private void ExpenseButtonClicked(object sender, RoutedEventArgs e)
        {
            var expenseForm = new ExpenseForm(this.dbAccess);
            expenseForm.Show();
        }

        private void ViewTransactionClicked(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < this.transactions.Count; i++)
            {
                if (this.transactions[i] is Income)
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
        }

        private void DeleteTransactionClicked(object sender, RoutedEventArgs e)
        {
            var sqlManager = this.dbAccess.GetDBConnection();
     
            for (var i = 0; i < this.transactions.Count; i++)
            {
                if (this.transactions[i] is Income)
                {
                    if (this.transactions[i].InvoiceNumber == (sender as Button).Tag.ToString())
                    {
                        //delete from db
                        sqlManager.DeleteFrom("Income", string.Format("NB_FAKTUR = '{0}'", this.transactions[i].InvoiceNumber));

                        //update transaction view
                        this.transactions.Remove(this.transactions[i]);
                        this.table.ItemsSource = this.transactions;
                    }
                }
                else
                {
                    if (this.transactions[i].InvoiceNumber == (sender as Button).Tag.ToString())
                    {
                        sqlManager.DeleteFrom("Expense", string.Format("NB_FAKTUR = '{0}'", this.transactions[i].InvoiceNumber));
                        this.transactions.Remove(this.transactions[i]);
                        this.table.ItemsSource = this.transactions;
                    }
                }
            }
            

        }
    }
}
