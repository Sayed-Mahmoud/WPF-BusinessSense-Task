using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFBusinessSense_Task
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
  System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location).Handle,
  Int32Rect.Empty,
  BitmapSizeOptions.FromEmptyOptions());

                this.Icon = imageSource;
            }
            catch
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
            System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location).Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

                this.Icon = imageSource;
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            //App.Current.Shutdown();
            //Environment.Exit(Environment.ExitCode);
        }

        private void ItemsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new FormItems()
            {
                Icon = this.Icon,
                Owner = this
            }.Show();
        }

        private void CashSalesInvoiceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new CashSalesInvoice()
            {
                Icon = this.Icon,
                Owner = this
            }.Show();
        }

        private void TotalSalesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new TotalSalesInquiry()
            {
                Icon = this.Icon,
                Owner = this
            }.Show();
        }
    }
}