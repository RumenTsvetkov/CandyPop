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
                data.ReportDocumentValues.Add("InvoiceNumber", this.income.InvoiceNumber);

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
