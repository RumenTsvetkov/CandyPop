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
    /// Interaction logic for IncomeReport.xaml
    /// </summary>
    public partial class IncomeReport : Window
    {
        private Income income;

        public IncomeReport(Income income)
        {
            InitializeComponent();

            this.income = income;
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            try
            {
                var reportDocument = new ReportDocument();

                StreamReader reader = new StreamReader(new FileStream(@"Templates\IncomeReportTemplate.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Template\");
                reader.Close();

                var data = new ReportData();                

                data.ReportDocumentValues.Add("Buyer", this.income.Buyer);
                data.ReportDocumentValues.Add("Address", this.income.Address);
                data.ReportDocumentValues.Add("TransactionDate", this.income.Date.ToShortDateString());
                data.ReportDocumentValues.Add("InvoiceNumber", this.income.InvoiceNumber);

                // table list
                var table = new DataTable("list");
                table.Columns.Add("No", typeof(string));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Quantity", typeof(int));
                table.Columns.Add("Price", typeof(float));
                table.Columns.Add("TotalPrice", typeof(float));

                for (var i = 0; i < this.income.Items.Count; i++)
                {
                    var item = this.income.Items[i];
                    table.Rows.Add(new object[] { i + 1, item.Name, item.Quantity, item.Price, item.Quantity * item.Price });
                }

                data.DataTables.Add(table);
                data.ReportDocumentValues.Add("GrandTotal", this.income.Total);

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
                documentViewer.Document = xps.GetFixedDocumentSequence();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\r\n\r\n" + error.GetType() + "\r\n" + error.StackTrace, error.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
