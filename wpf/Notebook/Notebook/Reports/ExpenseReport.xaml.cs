namespace Notebook.Reports
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using CodeReason.Reports;
    using System.Windows.Xps.Packaging;
    using Notebook.Model;
    using System.Data;

    /// <summary>
    /// Interaction logic for ExpenseReport.xaml
    /// </summary>
    public partial class ExpenseReport : Window
    {
        private Expense expense;

        public ExpenseReport(Expense expense)
        {
            InitializeComponent();

            this.expense = expense;
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            try
            {
                var reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Templates\ExpenseReportTemplate.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Template\");
                reader.Close();

                var data = new ReportData();                

                data.ReportDocumentValues.Add("Seller", this.expense.Seller);
                data.ReportDocumentValues.Add("Address", this.expense.Address);
                data.ReportDocumentValues.Add("TransactionDate", this.expense.Date.ToShortDateString());
                data.ReportDocumentValues.Add("InvoiceNumber", this.expense.InvoiceNumber);

                // table list
                var table = new DataTable("list");
                table.Columns.Add("No", typeof(string));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Quantity", typeof(int));
                table.Columns.Add("Price", typeof(float));
                table.Columns.Add("TotalPrice", typeof(float));

                for (var i = 0; i < this.expense.Items.Count; i++)
                {
                    var item = this.expense.Items[i];
                    table.Rows.Add(new object[] { i + 1, item.Name, item.Quantity, item.Price, item.Quantity * item.Price });
                }

                data.DataTables.Add(table);
                data.ReportDocumentValues.Add("GrandTotal", this.expense.Total);

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
