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
using Notebook.Model;
using CodeReason.Reports;
using System.IO;
using System.Windows.Xps.Packaging;

namespace Notebook.Reports
{
    /// <summary>
    /// Interaction logic for AccountReport.xaml
    /// </summary>
    public partial class AccountReport : Window
    {
        private List<Transactions> transactions;
        
        public AccountReport(List<Transactions> transactions)
        {
            InitializeComponent();

            this.transactions = transactions;
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            try
            {
                var reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Templates\AccountReportTemplate.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Template\");
                reader.Close();

                var data = new ReportData();

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
                this.documentViewer.Document = xps.GetFixedDocumentSequence();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\r\n\r\n" + error.GetType() + "\r\n" + error.StackTrace, error.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
