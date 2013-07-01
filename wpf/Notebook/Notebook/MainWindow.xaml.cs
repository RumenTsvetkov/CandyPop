namespace Notebook
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Notebook.ModelView;
    using Notebook.Model;
    using SQLDataAccessLayer;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbAccess dbAccess;

        public MainWindow()
        {
            InitializeComponent();

            this.dbAccess = new DbAccess();
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

        }
    }
}
