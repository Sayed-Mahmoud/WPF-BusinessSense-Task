using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using WPFBusinessSense_Task.Reports;

namespace WPFBusinessSense_Task
{
    public partial class TotalSalesInquiry : Window
    {
        //  private DataTable dataTable;
        private byte ShowType;

        public TotalSalesInquiry()
        {
            InitializeComponent();
        }

        private void TotalSalesInquiry_Loaded(object sender, RoutedEventArgs e)
        {
            FromDateTimePicker.SelectedDate = ToDateTimePicker.SelectedDate = DateTime.Now.Date;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            MyDataGridView.DataContext = null;

            using (SqlConnection connection = new SqlConnection(MyFunctions.MyConnection))
            {
                using (SqlCommand command = new SqlCommand("", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                })
                {
                    command.Parameters.AddRange(new SqlParameter[] {
                new SqlParameter("@FromDate", FromDateTimePicker.SelectedDate.Value.Date),
                new SqlParameter("@ToDate", ToDateTimePicker.SelectedDate.Value.Date) });
                    SqlDataAdapter adapter = new SqlDataAdapter();

                    using (DataTable table = new DataTable
                    {
                        Locale = System.Globalization.CultureInfo.InvariantCulture
                    })
                    {
                        if (ByItemRadioBtn.IsChecked == true)
                        {
                            ShowType = 0;
                            command.CommandText = "SelectTotalSalesByItem";
                        }
                        else
                        {
                            ShowType = 1;
                            command.CommandText = "TotalSalesByDate";
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                        // dataTable = table;
                        MyDataGridView.DataContext = table;
                    }
                }
            }
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGridView.DataContext != null)
            {
                new TotalSalesReport(MyDataGridView.DataContext, ShowType)//MyDataGridView.ItemsSource
                {
                    Icon = this.Icon,
                    Owner = this
                }.ShowDialog();
            }
        }

        private void TotalSalesInquiry_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.F)
            {
                if (SearchBtn.IsVisible && SearchBtn.IsEnabled)
                {
                    SearchBtn_Click(sender, null);
                }
            }
            else if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.P)
            {
                if (PrintBtn.IsVisible && PrintBtn.IsEnabled)
                {
                    PrintBtn_Click(sender, null);
                }
            }
        }
    }
}