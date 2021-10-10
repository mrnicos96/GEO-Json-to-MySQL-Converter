using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GEO_Json_to_MySQL_Converter.ViewModels
{
    public class BusyViewModel : INotifyPropertyChanged
    {
        public delegate void BusyStateChangeHandler(bool isBusy, string busyContent);
        public event BusyStateChangeHandler BusyStateChanged;

        protected bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            protected set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        protected string busyContent;
        public string BusyContent
        {
            get => busyContent;
            protected set
            {
                busyContent = value;
                OnPropertyChanged();
            }
        }

        public void OnBusy(string content = "Подождите")
        {
            BusyContent = content;
            IsBusy = true;
            BusyStateChanged?.Invoke(IsBusy, BusyContent);
        }

        public void OffBusy()
        {
            IsBusy = false;
            BusyContent = String.Empty;
            BusyStateChanged?.Invoke(IsBusy, BusyContent);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
