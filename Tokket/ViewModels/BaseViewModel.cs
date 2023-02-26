using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Tokket.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public CollectionView TokParameterUIList { get; set; }
        public Page PageInstance { get; set; }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public ICommand ClosePopupCommand { get; set; }

        public BaseViewModel() { }

        public BaseViewModel(Page page)
        {
            PageInstance = page;
            //ClosePopupCommand = new Command(async () => await PageInstance.Navigation.PopPopupAsync());
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion



    }
}
