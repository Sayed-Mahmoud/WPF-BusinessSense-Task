using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFBusinessSense_Task.Models;

namespace WPFBusinessSense_Task
{
    /// <summary>
    /// Interaction logic for CashSalesInvoice.xaml
    /// </summary>
    public partial class CashSalesInvoice : Window
    {
        private MyDbContext context;
        private CollectionViewSource invoicesViewSource;
        private CollectionViewSource invoicesInvoices_ItemsViewSource;
        /*
        CollectionViewSource itemsViewSource;
        CollectionViewSource invoices_ItemsViewSource;
        */

        public CashSalesInvoice()
        {
            InitializeComponent();
        }

        private void CashSalesInvoice1_Loaded(object sender, RoutedEventArgs e)
        {
            invoicesViewSource = (CollectionViewSource)this.FindResource("invoicesViewSource");
            invoicesInvoices_ItemsViewSource = (CollectionViewSource)this.FindResource("invoicesInvoices_ItemsViewSource");
            /*
            itemsViewSource = (CollectionViewSource)this.FindResource("itemsViewSource");
            invoices_ItemsViewSource = (CollectionViewSource)this.FindResource("invoices_ItemsViewSource");
            */
            context = new MyDbContext();
            context.Items.Load();
            context.Invoices.Load();
            context.Invoices_Items.Load();

            FromDateTimePicker.SelectedDate = DateTime.Now.AddMonths(-1).Date;
            ToDateTimePicker.SelectedDate = DateTime.Now.Date;

            ItemNameCBox.SelectedValuePath = ItemCodeCBox.SelectedValuePath = ItemIdCBox.SelectedValuePath = "ItemId";
            ItemIdCBox.DisplayMemberPath = "ItemId";
            ItemNameCBox.DisplayMemberPath = "ItemName";
            ItemCodeCBox.DisplayMemberPath = "ItemBarCode";

            ObservableCollection<Items> items = context.Items.Local;
            //itemsViewSource.Source = items;
            ItemNameCBox.ItemsSource = ItemCodeCBox.ItemsSource = ItemIdCBox.ItemsSource = items;

            SearchBtn_Click(sender, e);

            if (invoicesViewSource.Source == null)
            {
                invoicesViewSource.Source = new ObservableCollection<Invoices>();
            }
            invoicesViewSource.View.CurrentChanging += View_CurrentChanging;
            invoicesViewSource.View.CurrentChanged += View_CurrentChanged;

            /*
            if (invoicesInvoices_ItemsViewSource.Source == null)
            {
                invoicesInvoices_ItemsViewSource.Source = new ObservableCollection<Invoices_Items>();
            }
            invoicesInvoices_ItemsViewSource.View.CurrentChanging += InvoicesInvoices_ItemsViewSource_CurrentChanging;
            invoicesInvoices_ItemsViewSource.View.CurrentChanged += InvoicesInvoices_ItemsViewSource_CurrentChanged;
            */
            ItemIdCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
            ItemCodeCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
            ItemNameCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;

            MyDataGridView.SelectedCellsChanged += MyDataGridView_SelectedCellsChanged;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime fromdate = FromDateTimePicker.SelectedDate.Value.Date;
            DateTime todate = ToDateTimePicker.SelectedDate.Value.Date;
            invoicesViewSource.Source = context.Invoices.Local.Where(x => x.InvoiceDate.Date >= fromdate && x.InvoiceDate.Date <= todate);
            invoicesViewSource.View.Refresh();
        }

        private void SetTotals(bool SetItem, bool SetInvoice)
        {
            if (SetItem)
            {
                decimal.TryParse(QtyTBox.Text, out decimal qty);
                decimal.TryParse(PriceTBox.Text, out decimal price);

                TotalItemBox.Text = Math.Round(qty * price, 2).ToString();
                /*
                if (invoicesInvoices_ItemsViewSource.View != null && invoicesInvoices_ItemsViewSource.View.CurrentItem != null)
                {
                    Invoices_Items invoices_Items = invoicesInvoices_ItemsViewSource.View.CurrentItem as Invoices_Items;
                    if (invoices_Items.IsNewInvoiceItem)
                    {
                        //invoices_Items.Item_TotalPrice = Math.Round(invoices_Items.Quantity * invoices_Items.InvItem_SalesPrice, 2);
                    }
                    MessageBox.Show(invoices_Items.InvItem_SalesPrice.ToString());
                }
                else
                {
                    QtyTBox.Text = PriceTBox.Text = TotalItemBox.Text = "0";
                }*/
            }
            if (SetInvoice)
            {
                decimal Tot = 0;

                foreach (DataRowView row in MyDataGridView.Items)//DataGridRow DataRow
                {
                    var val = row.Row[ItemTotalPriceCol.DisplayIndex];
                    if (val != null)
                    {
                        decimal.TryParse(val.ToString(), out decimal total);
                        Tot += total;
                    }
                }

                TotalInvoicePrice.Text = Math.Round(Tot, 2).ToString();
            }
        }

        /*
        private void InvoicesInvoices_ItemsViewSource_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            ItemIdCBox.SelectionChanged -= ItemIdBarcodeNameCBox_SelectionChanged;
            ItemNameCBox.SelectionChanged -= ItemIdBarcodeNameCBox_SelectionChanged;
            ItemCodeCBox.SelectionChanged -= ItemIdBarcodeNameCBox_SelectionChanged;
        }
        private void InvoicesInvoices_ItemsViewSource_CurrentChanged(object sender, EventArgs e)
        {
            ItemIdCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
            ItemNameCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
            ItemCodeCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
        }
        */

        private void ItemIdBarcodeNameCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var CBox = sender as ComboBox;
            if (CBox.SelectedValue != null)
            {
                if (ItemIdCBox != CBox)
                {
                    ItemIdCBox.SelectionChanged -= ItemIdBarcodeNameCBox_SelectionChanged;
                    ItemIdCBox.SelectedValue = CBox.SelectedValue;
                    ItemIdCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
                }

                if (ItemNameCBox != CBox)
                {
                    ItemNameCBox.SelectionChanged -= ItemIdBarcodeNameCBox_SelectionChanged;
                    ItemNameCBox.SelectedValue = CBox.SelectedValue;
                    ItemNameCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
                }

                if (ItemCodeCBox != CBox)
                {
                    ItemCodeCBox.SelectionChanged -= ItemIdBarcodeNameCBox_SelectionChanged;
                    ItemCodeCBox.SelectedValue = CBox.SelectedValue;
                    ItemCodeCBox.SelectionChanged += ItemIdBarcodeNameCBox_SelectionChanged;
                }
            }

            if (CBox.SelectedIndex != -1)
            {
                int itemid = (int)CBox.SelectedValue;

                //Items item = context.Items.Where(x => x.ItemId == (int)CBox.SelectedValue).Single();
                PriceTBox.Text = context.Items.Where(x => x.ItemId == (int)CBox.SelectedValue).Single().ItemSalesPrice.ToString();//item.ItemSalesPrice.ToString();
                SetTotals(true, false);
            }
            else
            {
                PriceTBox.Text = "0";
            }
        }

        private void NewInvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewInvoice();
        }

        private Invoices RemoveThisInvoice = null;

        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (invoicesViewSource.View.CurrentItem is Invoices item && item.IsNewInvoice)
            {
                RemoveThisInvoice = item;
            }
            RemoveItem(true);
        }

        private void View_CurrentChanged(object sender, EventArgs e)
        {
            if (RemoveThisInvoice != null)
            {
                if (context.Invoices.Local.Contains(RemoveThisInvoice))
                {
                    context.Invoices.Remove(RemoveThisInvoice);
                }

                RemoveThisInvoice = null;
                invoicesViewSource.View.Refresh();
            }
        }

        private void AddNewInvoice()
        {
            //decimal.TryParse(InvoiceIdTBox.Text, out decimal invId);
            //if (!context.Invoices.Any(x => x.InvoiceId == invId && x.IsNew == true))
            if (!(invoicesViewSource.View.CurrentItem is Invoices item) || !item.IsNewInvoice)
            {
                DateTime dt = DateTime.Now;
                Invoices invoice = new Invoices
                {
                    InvoiceDate = dt,
                    InvoiceId = context.Invoices.Max(x => x.InvoiceId) + 1,
                    IsNewInvoice = true
                };
                context.Invoices.Add(invoice);
                invoicesViewSource.View.Refresh();
                invoicesViewSource.View.MoveCurrentTo(invoice);

                InvoicedateTimePicker.SelectedDate = dt;

                AddNewItem();
            }
        }

        public bool SaveInvoice(bool refresh = true)
        {
            /*
if (!(invoicesViewSource.View.CurrentItem is Invoices invoice))
{
    invoice = new Invoices()
    {
        InvoiceId = context.Invoices.Max(x => x.InvoiceId) + 1,
        InvoiceDate = InvoicedateTimePicker.SelectedDate.Value,
    };
    context.Invoices.Add(invoice);
}
*/
            if (invoicesViewSource.View == null || !(invoicesViewSource.View.CurrentItem is Invoices invoice))
            {
                MessageBox.Show("Please create a new Invoice first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (InvoicedateTimePicker.SelectedDate == null || !InvoicedateTimePicker.SelectedDate.HasValue)// || !DateTime.TryParse(InvoicedateTimePicker.Text, out DateTime invoiceDT))
            {
                MessageBox.Show("Please select a valid Invoice date and Time!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!MyDataGridView.HasItems && refresh)//invoicesInvoices_ItemsViewSource.View == null || invoicesInvoices_ItemsViewSource.Source == null ||
            {
                MessageBox.Show("Please add at least one item!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (invoice.Invoices_Items.Any(x => x.IsNewInvoiceItem) && !SaveItem(true))
            {
                return false;
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
                return false;
            }

            invoice.IsNewInvoice = false;

            if (refresh)
            {
                invoicesViewSource.View.Refresh();
                //invoicesInvoices_ItemsViewSource.View.Refresh();
            }
            return true;
        }

        private void SaveInvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveInvoice();
            // AddNewInvoice();
        }

        private void DeleteInvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (invoicesViewSource.View.CurrentItem is Invoices invoice)
            {
                if (MessageBox.Show("Are you sure?", "Delete confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    context.Invoices.Remove(invoice);
                    context.SaveChanges();
                    invoicesViewSource.View.Refresh();
                }
            }
        }

        private bool AddNewItem()
        {
            if (invoicesViewSource.View.CurrentItem is Invoices invoice)
            {
                if (invoicesInvoices_ItemsViewSource.View.CurrentItem == null || !(invoicesInvoices_ItemsViewSource.View.CurrentItem as Invoices_Items).IsNewInvoiceItem)
                {
                    Invoices_Items nitem = new Invoices_Items
                    {
                        IsNewInvoiceItem = true,
                        Invoice_Id = invoice.InvoiceId,
                        Quantity = 1,
                    };
                    // context.Invoices_Items.Add(nitem);
                    invoice.Invoices_Items.Add(nitem);
                    invoicesInvoices_ItemsViewSource.View.Refresh();
                    invoicesInvoices_ItemsViewSource.View.MoveCurrentTo(nitem);
                    return true;
                }
            }
            return false;
        }

        private void NewItemBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem();
        }

        public bool SaveItem(bool ForInvoice = false)
        {
            decimal price;
            decimal qty;
            if (!ForInvoice && invoicesViewSource.View == null || !(invoicesViewSource.View.CurrentItem is Invoices invoice))
            {
                MessageBox.Show("Please create a new Invoice first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (InvoicedateTimePicker.SelectedDate == null || !InvoicedateTimePicker.SelectedDate.HasValue)// || !DateTime.TryParse(InvoicedateTimePicker.Text, out DateTime invoiceDT))
            {
                MessageBox.Show("Please select a valid Invoice date and Time!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (ItemIdCBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid Item ID!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (ItemCodeCBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid Item Barcode!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (ItemNameCBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid Item Name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!decimal.TryParse(QtyTBox.Text, out qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid Quantity!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!decimal.TryParse(PriceTBox.Text, out price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid Price!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (invoicesInvoices_ItemsViewSource.View == null || !(invoicesInvoices_ItemsViewSource.View.CurrentItem is Invoices_Items item))
            {
                item = new Invoices_Items
                {
                    Invoice_Id = invoice.InvoiceId,
                    InvItem_SalesPrice = price,
                    Quantity = qty,
                    Item_Id = (int)ItemIdCBox.SelectedValue,
                    IsNewInvoiceItem = true
                };
                //context.Invoices_Items.Add(item);
                invoice.Invoices_Items.Add(item);
            }

            if (!ForInvoice)
            {
                if (!SaveInvoice(false))
                {
                    return false;
                }
            }
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
                return false;
            }
            item.IsNewInvoiceItem = false;

            if (!ForInvoice)
            {
                //invoicesViewSource.View.Refresh();
                invoicesInvoices_ItemsViewSource.View.Refresh();
                AddNewItem();
            }
            return true;
        }

        private void SaveItemBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveItem();
        }

        private void CashSalesInvoice1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.N)
            {
                if (NewInvoiceBtn.IsVisible && NewInvoiceBtn.IsEnabled)
                {
                    NewInvoiceBtn_Click(sender, null);
                }
            }
            else if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.S)
            {
                if (SaveInvoiceBtn.IsVisible && SaveInvoiceBtn.IsEnabled)
                {
                    SaveInvoiceBtn_Click(sender, null);
                }
            }
            else if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) && e.Key == Key.Delete)
            {
                if (DeleteInvoiceBtn.IsVisible && DeleteInvoiceBtn.IsEnabled)
                {
                    DeleteInvoiceBtn_Click(sender, null);
                }
            }
        }

        private void QtyPriceTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetTotals(true, false);
            MyDataGridView.UpdateLayout();
        }

        private void InvoicedateTimePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                DateTime _new = (DateTime)e.AddedItems[0];
                if (_new.Hour == 0 && _new.Minute == 0 && _new.Second == 0)
                {
                    DateTime _old = (DateTime)e.RemovedItems[0];

                    TimeSpan ts = new TimeSpan(_old.Hour, _old.Minute, _old.Second);
                    e.AddedItems[0] = _new + ts;
                    InvoicedateTimePicker.SelectedDate = _new + ts;
                    e.Handled = true;
                }
            }
        }

        private void RemoveItem(bool force = false)
        {
            int index = MyDataGridView.Items.IndexOf(MyDataGridView.SelectedItem);
            Invoices_Items[] _items = new Invoices_Items[MyDataGridView.Items.Count];
            MyDataGridView.Items.CopyTo(_items, 0);
            foreach (Invoices_Items item in _items)
            {
                if (!force && MyDataGridView.Items.IndexOf(item) == index)
                    continue;

                if (item.Id <= 0 && item.IsNewInvoiceItem)
                {
                    context.Invoices_Items.Remove(item);
                    MyDataGridView.UpdateLayout();
                }
            }
        }

        private void MyDataGridView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            RemoveItem();
        }
    }
}