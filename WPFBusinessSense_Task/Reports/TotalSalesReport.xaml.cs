using CrystalDecisions.CrystalReports.Engine;
using System.Windows;
using System.Windows.Input;

namespace WPFBusinessSense_Task.Reports
{
    public partial class TotalSalesReport : Window
    {
        private readonly byte ShowType;
        private readonly object MyDataSource;

        public TotalSalesReport(object MyDataSource, byte ShowType)
        {
            InitializeComponent();
            this.MyDataSource = MyDataSource;
            this.ShowType = ShowType;
        }

        private void TotalSalesReport1_Loaded(object sender, RoutedEventArgs e)
        {
            ReportClass totalSales;
            if (ShowType == 0)
            {
                totalSales = new TotalSalesByItem();
            }
            else
            {
                totalSales = new TotalSalesByDate();
            }
            totalSales.SetDataSource(this.MyDataSource);
            this.crystalReportViewer1.ViewerCore.ReportSource = totalSales;
        }

        private void TotalSalesReport1_KeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}