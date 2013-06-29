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

        private List<Income> FindIncomes(DateTime startTime, DateTime endTime)
        {
            return null;
        }
    }
}
