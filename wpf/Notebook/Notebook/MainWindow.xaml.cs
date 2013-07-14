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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbAccess dbAccess;

        private ObservableCollection<Transactions> incomes = new ObservableCollection<Transactions>();

        public MainWindow()
        {
            InitializeComponent();

            this.dbAccess = new DbAccess();
            
            // For test only
            var income = new Income(this.dbAccess)
            {
                Buyer = "PT.INTI CAHAYA BUANA",
                Address = "Komplek industri greenland ab no.46",
                Date = DateTime.Now,
                InvoiceNumber = "SG001"
            };
            this.incomes.Add(income);

            this.table.ItemsSource = this.incomes;
        }

        private void IncomeButtonClicked(object sender, RoutedEventArgs e)
        {
            var incomeForm = new IncomeForm(this.dbAccess);
            incomeForm.Show();
        }

        private List<Income> FindIncomes(DateTime startDate, DateTime endDate)
        {
            var income = new Income(this.dbAccess);
            var result = income.getFaktursByDates(startDate, endDate);
            
            var incomeList = new List<Income>();
            foreach (object record in result)
            {
                Dictionary<string, object> temp = (Dictionary<string, object>)record;
                var singleIncome = new Income(this.dbAccess);
                singleIncome.Load(temp["NB_FAKTUR"].ToString());
                incomeList.Add(singleIncome);
            }
            if (incomeList.Count() > 0) 
            {
                return incomeList;
            }
            return null;
        }

        private void FindClicked(object sender, RoutedEventArgs e)
        {
            var listIncomes = new List<Income>();
            //Income income = new Income(this.dbAccess);


            if (dStart.ToString() == "")
            {
                MessageBox.Show("Silah pilih tanggal mulai");

            }
            else if (dEnd.ToString() == "")
            {
                MessageBox.Show("Silah pilih tanggal akhir");

            }
            else if (dEnd.SelectedDate < dStart.SelectedDate)
            {

                MessageBox.Show("Silah pilih tanggal awal lebih kecil dr tanggal akhir");
            }

            else
            {
                this.incomes.Clear();
                listIncomes = FindIncomes((DateTime)dStart.SelectedDate, (DateTime)dEnd.SelectedDate);
                if (listIncomes != null)
                {
                    
                    foreach (Income singleIncome in listIncomes)
                    {
                        this.incomes.Add(singleIncome);
                        this.table.ItemsSource = this.incomes;

                    }
                }
                else
                {
                    MessageBox.Show("Tidak ditemukan daftar transaksi");
                }
                
                
            }

            

        }

        private void ExpenseButtonClicked(object sender, RoutedEventArgs e)
        {            
        }

        private void ViewTransactionClicked(object sender, RoutedEventArgs e)
        {
            Income selectedIncome = this.incomes.Where(
                i => (sender as Button).Tag.ToString() == (i as Income).InvoiceNumber).Single() as Income;
            var incomeReport = new IncomeReport(selectedIncome);
            incomeReport.Show();
        }

        private void sheet_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
