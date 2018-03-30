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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Windows.UI.Core;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using MyList.Model;

namespace MyList {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListPage : Page {
        public static ListPage Current;
        public ListPage() {
            this.InitializeComponent();
            this.ItemSelected = -1;
            Current = this;
            // test code
            
            ItemsDataSource.GetData().Add(new Item());
        }
        private int itemSelected;
        public int ItemSelected {
            get {
                return this.itemSelected;
            }
            set {
                itemSelected = value;
                toDoList.SelectedIndex = value;
            }
        }

        private void ListClick(object sender, ItemClickEventArgs e) {
            int index = ItemsDataSource.GetData().GetIndex(e.ClickedItem as Item);
            toDoList.SelectedIndex = index;
            GotoDetail();
        }

        private void GotoDetail() {
            if (Window.Current.Bounds.Width <= 800) {
                MainPage.Current.GoToNewPage();
            }
            ItemSelected = toDoList.SelectedIndex;
            if (ItemSelected != -1) {
                NewPage.Current.IsCreateStatus = false;
                NewPage.Current.ShowDetail(toDoList.SelectedIndex);
            } else {
                NewPage.Current.IsCreateStatus = true;
            }
        }

        private void DeleteItem_ItemInvoked(SwipeItem sender, SwipeItemInvokedEventArgs args) {
            Item data = (Item)args.SwipeControl.DataContext;
            if (data != null) {
                ItemsDataSource.GetData().Remove(data);
            }
        }
    }

}
