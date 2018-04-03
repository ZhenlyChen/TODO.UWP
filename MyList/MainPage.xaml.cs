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

namespace MyList {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page {
        public static MainPage Current;
        public bool DoneView;
        public Visibility ListVisibility {
            get { return ListFrame.Visibility; }
            set { ListFrame.Visibility = value; }
        }
        public MainPage() {
            DoneView = false;
            InitializeComponent();
            Current = this;
            ListFrame.Navigate(typeof(ListPage));
            NewFrame.Navigate(typeof(NewPage));
            this.SizeChanged += (s, e) => {
                if (ListFrame.Visibility == Visibility.Visible) {
                    SetRightAuto();
                }
                if (e.NewSize.Width > 800) {
                    ListFrame.Visibility = Visibility.Visible;
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        AppViewBackButtonVisibility.Collapsed;
                    MainPageGrid.ColumnDefinitions[1].Width = new GridLength(400);
                }
            };
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => {
                GoBackPage();
            };
            SetRightAuto();

            // SetBackground
            ImageBrush imageBrush = new ImageBrush {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Background1.jpg", UriKind.Absolute))
            };
            MainPageGrid.Background = imageBrush;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back) {
                RestoreData();
            }
        }

        public void RestoreData() {
            using (var db = new DataModel.DataContext()) {
                var tempItem = db.State.FirstOrDefault();
                if (tempItem != null) {
                    NewPage.Current.Restore(tempItem);
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

        private void SetLeftAuto() {
            MainPageGrid.ColumnDefinitions[0].Width = GridLength.Auto;
            MainPageGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }

        private void SetRightAuto() {
            MainPageGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MainPageGrid.ColumnDefinitions[1].Width = GridLength.Auto;
        }

        public void GoBackPage() {
            ListFrame.Visibility = Visibility.Visible;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
            SetRightAuto();
            if (IsSmallScreen()) {
                NewFrame.Visibility = Visibility.Collapsed;
            }
        }
        
        public void GoToNewPage() {
            if (IsSmallScreen()) {
                ListFrame.Visibility = Visibility.Collapsed;
                NewFrame.Visibility = Visibility.Visible;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SetLeftAuto();
            }
            NewPage.Current.SetDetail(-1);
        }

        private void Button_GotoNewPage(object sender, RoutedEventArgs e) {
            ListPage.Current.ItemSelected = -1;
            GoToNewPage();
        }

        private void Button_DeleteItem(object sender, RoutedEventArgs e) {
                ItemsDataSource.GetData().Remove(ListPage.Current.ItemSelected);
                ListPage.Current.ItemSelected = -1;
                GoBackPage();
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
