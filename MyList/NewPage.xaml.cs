using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;

namespace MyList {
    /// <summary>
    /// 新建页面
    /// </summary>
    public sealed partial class NewPage : Page {
        public static NewPage Current;
        public NewPage() {
            this.InitializeComponent();
            Current = this;
            this.IsCreateStatus = true;
        }
        private Boolean isCreateStatus;
        
        public Boolean IsCreateStatus {
            get {
                return this.isCreateStatus;
            }
            set {
                this.isCreateStatus = value;
                if (value == true) {
                    buttonCreate.Content = "Create";
                    titleBlock.Text = "Add a item";
                    ResetForm();
                } else {
                    buttonCreate.Content = "Update";
                    titleBlock.Text = "Edit the item";
                }
            }
        }

        public void ShowDetail(int index) {
            TodoList data = MainPage.Current.ListItemsData.TodoLists[index];
            ImageBox.Source = data.Icon;
            textBoxTitle.Text = data.Title;
            textBoxDes.Text = data.Des;
            DueDatePicker.Date = data.DueDate;
        }

        public void ResetForm() {
            textBoxTitle.Text = "";
            textBoxDes.Text = "";
            DueDatePicker.Date = DateTime.Now;
            BitmapImage Icon = new BitmapImage {
                UriSource = new Uri("ms-appx:/Assets/Square150x150Logo.scale-200.png")
            };
            ImageBox.Source = Icon;
        }

        private void SendAToast(string title, string content) {
            ToastVisual visual = new ToastVisual() {
                BindingGeneric = new ToastBindingGeneric() {
                    Children = {
                        new AdaptiveText() {
                            Text = title
                        },
                        new AdaptiveText() {
                            Text = content
                        },
                    }
                }
            };
            int conversationId = 384928;
            ToastContent toastContent = new ToastContent() {
                Visual = visual,
                Launch = new QueryString() {
                    { "conversationId", conversationId.ToString() }
                }.ToString()
            };
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastContent.GetXml()));
        }

        private async void SendADialog(string title, string content) {
            ContentDialog failDialog = new ContentDialog {
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                Title = title,
                Content = content
            };
            ContentDialogResult result = await failDialog.ShowAsync();
        }

        private void ClearText(object sender, RoutedEventArgs e) {
            if (this.IsCreateStatus == true) {
                ResetForm();
            } else if (ListPage.Current.ItemSelected != -1) {
                ShowDetail(ListPage.Current.ItemSelected);
            }
        }

        private TodoList getCurrentData() {
            return new TodoList() {
                Title = textBoxTitle.Text,
                Des = textBoxDes.Text,
                DueDate = DueDatePicker.Date,
                Icon = (BitmapImage)ImageBox.Source
            };
        }

        private void Create(object sender, RoutedEventArgs e) {
            if (textBoxTitle.Text == "" || textBoxDes.Text == "") {
                SendADialog("Create Fail", "Title and Description can not be empty!");
            } else if (DueDatePicker.Date < DateTime.Now.AddDays(-1)) {
                SendADialog("Create Fail", "DueDate is late to Now!");
            } else {
                if (this.isCreateStatus == true) {
                    SendAToast("Create Success", "You had create a event successfully.");
                    MainPage.Current.ListItemsData.AddItem(getCurrentData());
                    ResetForm();
                } else if (ListPage.Current.ItemSelected != -1) {
                    SendAToast("Update Success", "You had update a event successfully.");
                    MainPage.Current.ListItemsData.UpdateItem(getCurrentData(), ListPage.Current.ItemSelected);
                } else {
                    SendADialog("Update Failed!", "You have to select one.");
                }
                MainPage.Current.GoBackPage();
            }
        }

        private async void SelectPic_Click(object sender, RoutedEventArgs e) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                BitmapImage bitmap = new BitmapImage();
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                    bitmap.SetSource(stream);
                }
                ImageBox.Source = bitmap;
            }
        }

        private void Slider_Change(object sender, RangeBaseValueChangedEventArgs e) {
            PicViewBox.Width = e.NewValue;
            PicViewBox.Height = e.NewValue;
        }
    }
}
