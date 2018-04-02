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
using MyList.Model;

namespace MyList {
    /// <summary>
    /// 新建页面
    /// </summary>
    public sealed partial class NewPage : Page {
        public static NewPage Current;
        private Boolean isCreateStatus;
        private byte[] imageByte;
        private int editIndex;
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

        public NewPage() {
            this.InitializeComponent();
            Current = this;
            this.IsCreateStatus = true;
            imageByte = null;
        }

        public void SaveStatus() {
            var currentData = GetCurrentData();
            using (var db = new DataModel.DataContext()) {
                var tempItem = db.State.FirstOrDefault();
                if (tempItem != null) {
                    tempItem.Title = currentData.Title;
                    tempItem.Des = currentData.Des;
                    tempItem.DueDate = currentData.DueDate;
                    tempItem.Icon = currentData.ImageByte;
                    tempItem.ListIndex = editIndex;
                } else {
                    db.State.Add(new DataModel.TempState() {
                        Title = currentData.Title,
                        Des = currentData.Des,
                        DueDate = currentData.DueDate,
                        Icon = currentData.ImageByte,
                        ListIndex = editIndex
                    });
                }
                db.SaveChanges();
            }
        }

        public async void Restore(DataModel.TempState data) {
            // 异步操作，有点困难。需要在数据库读取之后并且列表生成之后执行。
            // SetDetail(data.ListIndex);
            if (data.ListIndex == -1) { // 暂时只恢复新建时候的状态，更新时候的状态以后再做
                editIndex = data.ListIndex;
                IsCreateStatus = true;
                ResetForm();
                ImageBox.Source = await Model.UtilTool.ConvertByteToImage(data.Icon);
                textBoxTitle.Text = data.Title;
                textBoxDes.Text = data.Des;
                DueDatePicker.Date = data.DueDate;
            } else {
                // IsCreateStatus = false;
                // ShowDetail();
            }
            // ListPage.Current.ItemSelected = data.ListIndex;
        }

        public void SetDetail(int index) {
            editIndex = index;
            if (index == -1) {
                IsCreateStatus = true;
                ResetForm();
            } else {
                IsCreateStatus = false;
                ShowDetail();
            }
        }

        public void ShowDetail() {
            Item data = ItemsDataSource.GetData().Get(editIndex);
            ImageBox.Source = data.Icon;
            textBoxTitle.Text = data.Title;
            textBoxDes.Text = data.Des;
            DueDatePicker.Date = data.DueDate;
        }

        public void ResetForm() {
            textBoxTitle.Text = "";
            textBoxDes.Text = "";
            DueDatePicker.Date = DateTime.Now;
            ImageBox.Source = new BitmapImage {
                UriSource = new Uri("ms-appx:///Assets/itemIcon.jpg")
            };
            imageByte = null;
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
            } else if (editIndex != -1) {
                ShowDetail();
            }
        }

        private Item GetCurrentData() {
            return new Item() {
                Title = textBoxTitle.Text,
                Des = textBoxDes.Text,
                DueDate = DueDatePicker.Date,
                Icon = (BitmapImage)ImageBox.Source,
                ImageByte = imageByte
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
                    ItemsDataSource.GetData().Add(GetCurrentData());
                    ResetForm();
                } else if (editIndex != -1) {
                    SendAToast("Update Success", "You had update a event successfully.");
                    ItemsDataSource.GetData().Update(GetCurrentData(), editIndex);
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
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                imageByte = await Model.UtilTool.ConvertImageToByte(file);
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
