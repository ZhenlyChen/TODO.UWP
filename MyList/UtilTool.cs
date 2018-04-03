using MyList.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList {
   public class UtilTool {
        public static async Task<BitmapImage> ConvertByteToImage(byte[] imageBytes) {
            if (imageBytes != null) {
                MemoryStream stream = new MemoryStream(imageBytes);
                var randomAccessStream = new MemoryRandomAccessStream(stream);
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(randomAccessStream);
                return bitmapImage;
            } else {
                return Item.DefaultIcon;
            }
        }

        public static async Task<byte[]> ConvertImageToByte(StorageFile file) {
            using (var inputStream = await file.OpenSequentialReadAsync()) {
                var readStream = inputStream.AsStreamForRead();
                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }

        public static async void SendADialog(string title, string content) {
            ContentDialog failDialog = new ContentDialog {
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                Title = title,
                Content = content
            };
            ContentDialogResult result = await failDialog.ShowAsync();
        }
    }
}
