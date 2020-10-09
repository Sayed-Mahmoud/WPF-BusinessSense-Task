using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WPFBusinessSense_Task.Models;

namespace WPFBusinessSense_Task
{
    public partial class FormItems : Window
    {
        private MyDbContext context;
        private CollectionViewSource itemsViewSource;

        public FormItems()
        {
            InitializeComponent();
        }

        private void FormItems_Loaded(object sender, RoutedEventArgs e)
        {
            SearchType.SelectedValuePath = "Id";
            SearchType.DisplayMemberPath = "Val";
            SearchType.ItemsSource = new List<CBoxItems>()
            {
                    new CBoxItems(0, "Item ID"),
                    new CBoxItems(1, "Item BarCode"),
                    new CBoxItems(2, "Item Name")
            };
            SearchType.SelectedIndex = 0;

            itemsViewSource = ((CollectionViewSource)this.FindResource("itemsViewSource"));
            context = new MyDbContext();
            context.Items.Load();
            itemsViewSource.Source = context.Items.Local;

            if (!MyDataGridView.HasItems)
            {
                AddNew();
            }

            itemsViewSource.View.CurrentChanging += View_CurrentChanging;
            itemsViewSource.View.CurrentChanged += View_CurrentChanged;
            SearchType.SelectionChanged += SearchType_SelectionChanged;

            //Window.GetWindow(this)
        }

        private Items RemoveThisitem = null;

        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            Items item = (Items)itemsViewSource.View.CurrentItem;
            if (item != null && item.ItemId == 0)
            {
                RemoveThisitem = item;
            }
        }

        private void View_CurrentChanged(object sender, EventArgs e)
        {
            if (RemoveThisitem != null && RemoveThisitem.ItemId == 0)
            {
                context.Items.Remove(RemoveThisitem);
                RemoveThisitem = null;
                itemsViewSource.View.Refresh();
            }
        }

        private void AddNew()
        {
            if (ItemIdTBox.Text != "0")
            {
                //MyDataGridView.Items.Add(new DataGridRow());
                //Items item = new Items();
                context.Items.Add(new Items());
                itemsViewSource.View.Refresh();
                itemsViewSource.View.MoveCurrentToLast();
            }
        }

        private static readonly Regex regex = new Regex(@"^(\d+(\,\d{3})*(\.\d{1,2})?)?$");//Regex("[^0-9.-]+");//Regex("[^0-9]+"); //

        private static bool IsTextAllowed(object sender, string text)
        {
            TextBox txtbox = (TextBox)sender;
            bool val = regex.IsMatch(text);
            if (!val && !string.IsNullOrWhiteSpace(text) && text.EndsWith(".") && text.Split('.').Count() == 2)
            {
                string ov = text.Remove(text.Length - 1);
                if (regex.IsMatch(ov))
                {
                    txtbox.Background = Brushes.Red;
                    return false;
                }
            }
            if (txtbox.Background == Brushes.Red)
            {
                txtbox.Background = Brushes.White;
            }

            return !val;
        }

        private void ItemPriceNum_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            TextBox txtbox = (TextBox)sender;
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(sender, txtbox.Text + text) || txtbox.Text.Contains(".") && text.Contains("."))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void ItemPriceNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox txtbox = (TextBox)sender;
            e.Handled = IsTextAllowed(sender, txtbox.Text + e.Text);
        }

        private void ItemPriceNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsTextAllowed(sender, ((TextBox)sender).Text);
        }

        private void ItemPriceNum_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtbox = (TextBox)sender;
            if (txtbox.Background == Brushes.Red)
            {
                txtbox.Text += "00";
                txtbox.Background = Brushes.White;
            }
        }

        private void AddCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            AddNew();
        }

        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (ItemBarcodeTBox.Text.Length < 1)
            {
                MessageBox.Show("Please enter a valid Barcode!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (ItemNameTBOX.Text.Length < 1)
            {
                MessageBox.Show("Please enter a valid name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!decimal.TryParse(ItemPriceNum.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (price <= 0)
            {
                MessageBox.Show("Please enter a valid price!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                /*
                string id = ItemIdTBox.Text;
                if (string.IsNullOrEmpty(id) || id == "0")
                {
                    Items newItem = new Items
                    {
                        ItemBarCode = ItemBarcodeTBox.Text,
                        ItemName = ItemNameTBOX.Text,
                        ItemSalesPrice = price,
                    };
                    context.Items.Add(newItem);
                }
                else
                {
                    Items item = context.Items.Where(x => x.ItemId.ToString() == id).SingleOrDefault();
                    item.ItemBarCode = ItemBarcodeTBox.Text;
                    item.ItemName = ItemNameTBOX.Text;
                    item.ItemSalesPrice = price;
                }
                */
                context.SaveChanges();
                itemsViewSource.View.Refresh();
                AddNew();
            }
        }

        private void DeleteItemCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            string id = ItemIdTBox.Text;
            if (string.IsNullOrEmpty(id) || itemsViewSource.View.CurrentItem == null)
            {
                return;
            }

            if (MessageBox.Show("Are you sure?", "Delete confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (string.IsNullOrEmpty(id) || id == "0")
                {
                    //itemsViewSource.View.MoveCurrentToFirst();
                    context.Items.Remove((Items)itemsViewSource.View.CurrentItem);
                    itemsViewSource.View.Refresh();
                    return;
                }
                else
                {
                    Items item = context.Items.Where(x => x.ItemId.ToString() == id).SingleOrDefault();
                    context.Items.Remove(item);
                    context.SaveChanges();
                    itemsViewSource.View.Refresh();
                }

                if (MyDataGridView.Items.Count < 1)
                {
                    AddNew();
                }
            }
        }

        private void SearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchTxt_TextChanged(sender, null);
        }

        private void SearchTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (itemsViewSource == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(SearchTxt.Text))
            {
                itemsViewSource.View.Filter = null;
                return;
            }

            if ((int?)SearchType.SelectedValue == 0)
            {
                //itemsViewSource.View.Filter
                MyDataGridView.Items.Filter = item => ((Items)item).ItemId.ToString().Contains(SearchTxt.Text);
            }
            else if ((int?)SearchType.SelectedValue == 1)
            {
                MyDataGridView.Items.Filter = item => ((Items)item).ItemBarCode.ToString().ToLower().Contains(SearchTxt.Text.ToLower());
            }
            else if ((int?)SearchType.SelectedValue == 2)
            {
                MyDataGridView.Items.Filter = item => ((Items)item).ItemName.ToString().ToLower().Contains(SearchTxt.Text.ToLower());
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.N)
            {
                if (NewBtn.IsVisible && NewBtn.IsEnabled)
                    AddCommandHandler(sender, null);
            }
            else if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.S)
            {
                if (SaveBtn.IsVisible && SaveBtn.IsEnabled)
                    UpdateCommandHandler(sender, null);
            }
            else if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.F)
            {
                if (SearchTxt.IsVisible && SearchTxt.IsEnabled)
                    SearchTxt.Focus();
            }
            else if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) && e.Key == Key.Delete)
            {
                if (DeleteBtn.IsVisible && DeleteBtn.IsEnabled)
                    DeleteItemCommandHandler(sender, null);
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            MessageBox.Show(e.Item.GetType().ToString() + Environment.NewLine + e.Item.ToString());
            e.Accepted = true;
            /*
            if (string.IsNullOrEmpty(SearchTxt.Text))
            {
                if (Itemlist.Filter != null)
                    Itemlist.Filter = null;
                //MyDataGridView.Items.Filter = null;
                //selectItemsBindingSource.RemoveFilter();
                return;
            }

            if ((int?)SearchType.SelectedValue == 0)
            {/*
                myDataSet.SelectItems.
                Itemlist.Filter
                */
            /* MyDataGridView.Items.Filter*/ // = itemIdColumn => ((object)itemIdColumn).ToString().ToLower().Contains(SearchTxt.Text.ToLower());//"Convert(ItemId, 'System.String') LIKE ('%" + SearchTxt.Text + "%')";
        }
    }

    public class CBoxItems
    {
        private readonly int? id;
        private readonly string val;

        public CBoxItems(int? Id, string Val)
        {
            this.id = Id;
            this.val = Val;
        }

        public int? Id
        {
            get
            {
                return id;
            }
        }

        public string Val
        {
            get
            {
                return val;
            }
        }
    }
}