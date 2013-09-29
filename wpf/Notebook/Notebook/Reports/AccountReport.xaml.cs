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
using System.Data;

namespace Notebook.Reports
{
    /// <summary>
    /// Interaction logic for AccountReport.xaml
    /// </summary>
    public partial class AccountReport : Window
    {
        private List<Transactions> transactions;

        private float creditBalance;

        private float debitBalance;
        
        public AccountReport(List<Transactions> transactions, float creditBalance, float debitBalance)
        {
            InitializeComponent();

            this.transactions = transactions;
            this.creditBalance = creditBalance;
            this.debitBalance = debitBalance;
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

                // table list
                var table = new DataTable("transactions");
                table.Columns.Add("Date", typeof(DateTime));
                table.Columns.Add("InvoiceNo", typeof(string));
                table.Columns.Add("Debit", typeof(string));
                table.Columns.Add("Credit", typeof(string));

                foreach (var transaction in this.transactions)
                {
                    table.Rows.Add(new object[] { transaction.Date, transaction.InvoiceNumber, transaction.Debit, transaction.Credit });
                }

                data.DataTables.Add(table);
                data.ReportDocumentValues.Add("CreditBalance", this.creditBalance);
                data.ReportDocumentValues.Add("DebitBalance", this.debitBalance);
                data.ReportDocumentValues.Add("Balance", this.creditBalance - this.debitBalance);

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
