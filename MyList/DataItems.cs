using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList {
    public class DataItems {
        private ObservableCollection<TodoList> todoLists = new ObservableCollection<TodoList>();
        public ObservableCollection<TodoList> TodoLists { get { return this.todoLists; } }
        public void AddItem(TodoList newItem) {
            this.todoLists.Add(newItem);
        }
        public bool UpdateItem(TodoList item, int index) {
            if (index < 0 || index >= this.todoLists.Count) return false;
            this.todoLists[index] = item;
            return true;
        }
        public bool RemoveItem(int index) {
            if (index < 0 || index >= this.todoLists.Count) return false;
            this.todoLists.RemoveAt(index);
            return true;
        }
        public DataItems() {
        }
    }

    public class TodoList : BindableBase {
        private string title;
        private string des;
        private DateTimeOffset dueDate; 
        private bool done;
        private Visibility deleteLine;
        private BitmapImage icon;

        public TodoList() {
            this.title = "Item X";
            this.des = "This is Item X!";
            this.done = false;
            this.deleteLine = Visibility.Collapsed;
            this.dueDate = DateTime.Now;
            this.icon = new BitmapImage {
                UriSource = new Uri("ms-appx:/Assets/Square150x150Logo.scale-200.png")
            };
        }

        public BitmapImage Icon {
            get { return this.icon; }
            set { this.SetProperty(ref this.icon, value); }
        }

        public bool Done {
            get { return this.done; }
            set {
                this.SetProperty(ref this.done, value);
                this.DeleteLine = (this.done == false ? Visibility.Collapsed : Visibility.Visible);
            }
        }
        public string Title {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }
        public DateTimeOffset DueDate {
            get { return this.dueDate; }
            set { this.SetProperty(ref this.dueDate, value); }
        }
        public string Des {
            get { return this.des; }
            set { this.SetProperty(ref this.des, value); }
        }
        public Visibility DeleteLine {
            get { return this.deleteLine; }
            set { this.SetProperty(ref this.deleteLine, value); }
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
