using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Windows.UI.Core;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Input;
using MyList.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.Storage;

namespace MyList {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListPage : Page {
        public static ListPage Current;
        public ListPage() {
            this.InitializeComponent();
            Current = this;
        }
        public int ItemSelected {
            get {
                return toDoList.SelectedIndex;
            }
            set {
                NewPage.Current.SetDetail(value);
                toDoList.SelectedIndex = value;
            }
        }

        private void ListClick(object sender, ItemClickEventArgs e) {
            int index = ItemsDataSource.GetData().GetIndex(e.ClickedItem as Item);
            ItemSelected = index;
            MainPage.Current.State = "Detail";
        }

        private void DeleteItem_ItemInvoked(SwipeItem sender, SwipeItemInvokedEventArgs args) {
            Item data = (Item)args.SwipeControl.DataContext;
            if (data != null) {
                ItemsDataSource.GetData().Remove(data);
                ItemSelected = -1;
            }
        }

        private void MenuEdit_Click(object sender, RoutedEventArgs e) {
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            int index = ItemsDataSource.GetData().GetIndex(originalSource.DataContext as Item);
            ItemSelected = index;
            MainPage.Current.State = "Detail";
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e) {
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            Item data = (Item)originalSource.DataContext;
            if (data != null) {
                ItemsDataSource.GetData().Remove(data);
                ItemSelected = -1;
            }
        }

        // 分享模块
        private void MenuShare_Click(object sender, RoutedEventArgs e) {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            Item data = (Item)originalSource.DataContext;
            dataTransferManager.DataRequested += async (s, args) => {
                DataRequest request = args.Request;
                request.Data.Properties.Title = data.Title;
                request.Data.Properties.Description = "Share your todo item";
                RandomAccessStreamReference bitmap;
                if (data.ImageByte != null) {
                    InMemoryRandomAccessStream stream = await UtilTool.ConvertByteToStream(data.ImageByte);
                    bitmap = RandomAccessStreamReference.CreateFromStream(stream);
                } else {
                    bitmap = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/orange.png"));
                }
                request.Data.SetBitmap(bitmap);
                request.Data.SetText(data.Des + "\n" + data.DueDate.ToString("D"));
            };
            DataTransferManager.ShowShareUI();
        }

        // 搜索模块
        private void QueryItem(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            var text = args.QueryText;
            searchBox.Text = "";
            using (var db = new DataModel.DataContext()) {
                var items = db.Items.Where(b => b.Title.Contains(text) ||
                                                b.Des.Contains(text) ||
                                                b.DueDate.ToString("D").Contains(text))
                                          .ToList();
                string content = "";
                foreach (var item in items) {
                    content += $"{item.Title} {item.Des} {item.DueDate.ToString("D")}\n";
                }
                if (content.Equals("")) {
                    content = "No Result";
                }
                UtilTool.SendADialog("Search Result", content);
            }
        }

        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            var text = sender.Text;
            foreach (var item in ItemsDataSource.GetData().Source) {
                if (text.Equals("")) {
                    item.ShowIt = Visibility.Visible;
                    continue;
                }
                if (item.Title.Contains(text) ||
                    item.Des.Contains(text) ||
                    item.DueDate.ToString("D").Contains(text)) {
                    item.ShowIt = Visibility.Visible;
                } else {
                    item.ShowIt = Visibility.Collapsed;
                }
            }
        }
    }

}
