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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using MyList.Model;
using Windows.Storage;

namespace MyList {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page {
        public static MainPage Current;

        public MainPage() {
            State = "New";
            InitializeComponent();
            Current = this;
            ListFrame.Navigate(typeof(ListPage));
            NewFrame.Navigate(typeof(NewPage));

            this.SizeChanged += (s, e) => {
                if (e.NewSize.Width > 800) {
                    State = "All";
                } else {
                    if (State != "Detail") {
                        State = "List";
                    }
                }
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => {
                if (IsSmallScreen()) {
                    State = "List";
                }
                ListPage.Current.ItemSelected = -1;
            };

            // SetBackground
            ImageBrush imageBrush = new ImageBrush {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Background1.jpg", UriKind.Absolute))
            };
            MainPageGrid.Background = imageBrush;

            // 恢复挂起状态
            if (((App)App.Current).isSuspend) {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AddNewItem")) {
                    var isEditState = (bool)ApplicationData.Current.LocalSettings.Values["AddNewItem"];
                    if (isEditState) {
                        State = "Detail";
                    }
                }
            }

            if (State == "New") {
                if (IsSmallScreen()) {
                    State = "List";
                } else {
                    State = "All";
                }
            }
        }

        // 自适应状态管理
        private string state;
        public string State {
            get { return state; }
            set {
                ChangeState(value);
                state = value;
            }
        }
        private void ChangeState(string newState) {
            if (newState == state) {
                return;
            }
            switch (newState) {
                case "List":
                    ListFrame.Visibility = Visibility.Visible;
                    NewFrame.Visibility = Visibility.Collapsed;
                    ApplicationData.Current.LocalSettings.Values["AddNewItem"] = false;
                    MainPageGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    MainPageGrid.ColumnDefinitions[1].Width = new GridLength(0);
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        AppViewBackButtonVisibility.Collapsed;
                    break;
                case "Detail":
                    ListFrame.Visibility = Visibility.Collapsed;
                    NewFrame.Visibility = Visibility.Visible;
                    MainPageGrid.ColumnDefinitions[0].Width = new GridLength(0);
                    MainPageGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        AppViewBackButtonVisibility.Visible;
                    break;
                case "All":
                    NewFrame.Visibility = Visibility.Visible;
                    ListFrame.Visibility = Visibility.Visible;
                    ApplicationData.Current.LocalSettings.Values["AddNewItem"] = false;
                    MainPageGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    MainPageGrid.ColumnDefinitions[1].Width = new GridLength(400);
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        AppViewBackButtonVisibility.Collapsed;
                    break;
            }
        }

        // 状态的保存与恢复
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back) {
                using (var db = new DataModel.DataContext()) {
                    var tempItem = db.State.FirstOrDefault();
                    if (tempItem != null) {
                        NewPage.Current.Restore(tempItem);
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);
            NewPage.Current.SaveStatus();
        }

        private bool IsSmallScreen() {
            return Window.Current.Bounds.Width <= 800;
        }

        private void Button_GotoNewPage(object sender, RoutedEventArgs e) {
            ListPage.Current.ItemSelected = -1;
            if (IsSmallScreen()) {
                State = "Detail";
                ApplicationData.Current.LocalSettings.Values["AddNewItem"] = true;
            }
        }

        private void Button_DeleteItem(object sender, RoutedEventArgs e) {
            ItemsDataSource.GetData().Remove(ListPage.Current.ItemSelected);
            ListPage.Current.ItemSelected = -1;
            if (IsSmallScreen()) {
                State = "List";
            }
        }

        private void ChangeBackground(object sender, SelectionChangedEventArgs e) {
            ImageBrush imageBrush = new ImageBrush {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Background" +
                BackgroundList.SelectedIndex.ToString() + ".jpg", UriKind.Absolute))
            };
            MainPageGrid.Background = imageBrush;
        }
    }


}
