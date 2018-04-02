using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyList.Model {


    public class ItemsDataSource {
        private static ItemsDataSource data;

        public static ItemsDataSource GetData() {
            if (data == null) {
                data = new ItemsDataSource();
            }
            return data;
        }

        public ItemsDataSource() {
            Source = new ObservableCollection<Item>();
        }

        public static ObservableCollection<Item> ViewModel() {
            return GetData().Source;
        }

        public ObservableCollection<Item> GetSource() {
            return Source;
        }

        public ObservableCollection<Item> Source;

        public Item Get(int index) {
            if (index < 0 || index >= Source.Count) return null;
            return Source[index];
        }

        public int GetIndex(Item item) {
            return Source.IndexOf(item);
        }

        public void Add(Item newItem) {
            Source.Add(newItem);
            using (var db = new DataModel.DataContext()) {
                db.Add(newItem.ToModel());
                db.SaveChanges();
            }
        }

        public bool Update(Item newItem, int index) {
            if (index < 0 || index >= Source.Count) return false;
            Source[index] = Source[index].GetNewItem(newItem);
            var newDate = Source[index].ToModel();
            using (var db = new DataModel.DataContext()) {
                var item = db.Items.Single(b => b.ListId == newDate.ListId);
                item.Update(newDate);
                db.SaveChanges();
            }
            return true;
        }

        public bool Remove(int index) {
            if (index < 0 || index >= Source.Count) return false;
            using (var db = new DataModel.DataContext()) {
                var item = db.Items.Single(b => b.ListId == Source[index].GetId());
                db.Items.Remove(item);
                db.SaveChanges();
            }
            Source.RemoveAt(index);
            return true;
        }

        public bool Remove(Item item) {
            return Remove(Source.IndexOf(item));
        }

        public async void InitFromDB() {
            using (var db = new DataModel.DataContext()) {
                var items = db.Items.ToList();
                foreach (var item in items) {
                    Source.Add(await new Item().FromModel(item));
                }
            }
        }
    }

    public class Item : BindableBase {
        private DataModel.ListItem data;
        private string title;
        private string des;
        private Boolean isCheck;
        private DateTimeOffset dueDate;
        private BitmapImage icon;
        private byte[] imageByte;

        public async void InitItem() {
            data = new DataModel.ListItem {
                Title = "Item X",
                Des = "This is Item X",
                IsCheck = false,
                DueDate = DateTime.Now,
                Icon = null
            };
            await FromModel();
        }

        public Item() {
            InitItem();
        }

        public Item GetNewItem(Item newData) {
            Title = newData.Title;
            Des = newData.Des;
            IsCheck = newData.IsCheck;
            DueDate = newData.dueDate;
            Icon = newData.icon;
            ImageByte = newData.imageByte;
            return this;
        }

        public DataModel.ListItem ToModel() {
            data.Title = title;
            data.Des = des;
            data.IsCheck = isCheck;
            data.DueDate = dueDate;
            data.Icon = imageByte;
            return data;
        }

        public int GetId() {
            return data.ListId;
        }

        public async Task<Item> FromModel(DataModel.ListItem item = null) {
            if (item != null) {
                data = item;
            }
            if (data != null) {
                title = data.Title;
                des = data.Des;
                isCheck = data.IsCheck;
                dueDate = data.DueDate;
                if (data.Icon == null) {
                    icon = new BitmapImage(new Uri("ms-appx:///Assets/itemIcon.jpg"));
                } else {
                    imageByte = data.Icon;
                    icon = await UtilTool.ConvertByteToImage(imageByte);
                }
            }
            return this;
        }

        public Boolean IsCheck {
            get { return this.isCheck; }
            set {
                using (var db = new DataModel.DataContext()) {
                    var item = db.Items.Single(b => b.ListId == GetId());
                    item.IsCheck = value;
                    db.SaveChanges();
                }
                SetProperty(ref this.isCheck, value);
            }
        }
        public BitmapImage Icon {
            get { return this.icon; }
            set { SetProperty(ref this.icon, value); }
        }
        public string Title {
            get { return this.title; }
            set { SetProperty(ref this.title, value); }
        }
        public string Des {
            get { return this.des; }
            set { SetProperty(ref this.des, value); }
        }
        public DateTimeOffset DueDate {
            get { return this.dueDate; }
            set { SetProperty(ref this.dueDate, value); }
        }
        public byte[] ImageByte {
            get { return this.imageByte; }
            set { imageByte = value; }
        }
    }

    public class UtilTool {
        public static async Task<BitmapImage> ConvertByteToImage(byte[] imageBytes) {
            if (imageBytes != null) {
                MemoryStream stream = new MemoryStream(imageBytes);
                var randomAccessStream = new MemoryRandomAccessStream(stream);
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(randomAccessStream);
                return bitmapImage;
            } else {
                return new BitmapImage(new Uri("ms-appx:///Assets/itemIcon.jpg"));
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
    }

    public class CheckBoxToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType,
            object parameter, string language) {
            Boolean isChecked = (Boolean)value;
            Visibility line = isChecked ? Visibility.Visible : Visibility.Collapsed;
            return line;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language) {
            throw new NotImplementedException();
        }
    }


    public abstract class BindableBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null) {
            if (object.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class MemoryRandomAccessStream : IRandomAccessStream {
        private Stream m_InternalStream;

        public MemoryRandomAccessStream(Stream stream) {
            this.m_InternalStream = stream;
        }

        public MemoryRandomAccessStream(byte[] bytes) {
            this.m_InternalStream = new MemoryStream(bytes);
        }

        public IInputStream GetInputStreamAt(ulong position) {
            this.m_InternalStream.Seek((long)position, SeekOrigin.Begin);

            return this.m_InternalStream.AsInputStream();
        }

        public IOutputStream GetOutputStreamAt(ulong position) {
            this.m_InternalStream.Seek((long)position, SeekOrigin.Begin);

            return this.m_InternalStream.AsOutputStream();
        }

        public ulong Size {
            get { return (ulong)this.m_InternalStream.Length; }
            set { this.m_InternalStream.SetLength((long)value); }
        }

        public bool CanRead {
            get { return true; }
        }

        public bool CanWrite {
            get { return true; }
        }

        public IRandomAccessStream CloneStream() {
            throw new NotSupportedException();
        }

        public ulong Position {
            get { return (ulong)this.m_InternalStream.Position; }
        }

        public void Seek(ulong position) {
            this.m_InternalStream.Seek((long)position, 0);
        }

        public void Dispose() {
            this.m_InternalStream.Dispose();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options) {
            var inputStream = this.GetInputStreamAt(0);
            return inputStream.ReadAsync(buffer, count, options);
        }

        public Windows.Foundation.IAsyncOperation<bool> FlushAsync() {
            var outputStream = this.GetOutputStreamAt(0);
            return outputStream.FlushAsync();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer) {
            var outputStream = this.GetOutputStreamAt(0);
            return outputStream.WriteAsync(buffer);
        }
    }
}
