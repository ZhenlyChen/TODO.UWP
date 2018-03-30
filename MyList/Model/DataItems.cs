using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList.Model {
    public class ItemsDataSource {
        private static ItemsDataSource data;

        public static ItemsDataSource GetData() {
            if (data == null) {
                data = new ItemsDataSource();
            }
            return data;
        }

        public static ObservableCollection<Item> ViewModel() {
            return GetData().Source;
        }
        
        public ObservableCollection<Item> GetSource() {
            return Source;
        }

        public ObservableCollection<Item> Source { get; } = new ObservableCollection<Item>();

        public Item Get(int index) {
            if (index < 0 || index >= Source.Count) return null;
            return Source[index];
        }

        public int GetIndex(Item item) {
            return Source.IndexOf(item);
        }

        public void Add(Item newItem) {
            Source.Add(newItem);
        }

        public bool Update(Item item, int index) {
            if (index < 0 || index >= Source.Count) return false;
            Source[index] = item;
            return true;
        }

        public bool Remove(int index) {
            if (index < 0 || index >= Source.Count) return false;
            Source.RemoveAt(index);
            return true;
        }

        public bool Remove(Item item) {
            Source.Remove(item);
            return true;
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

    public class Item : BindableBase {
        private string title;
        private string des;
        private Boolean isCheck;
        private DateTimeOffset dueDate;
        private BitmapImage icon;

        public Item() {
            this.title = "Item X";
            this.des = "This is Item X!";
            this.isCheck = false;
            this.dueDate = DateTime.Now;
            this.icon = new BitmapImage {
                UriSource = new Uri("ms-appx:/Assets/Square150x150Logo.scale-200.png")
            };
        }

        public Boolean IsCheck {
            get { return this.isCheck; }
            set { SetProperty(ref this.isCheck, value); }
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
}
