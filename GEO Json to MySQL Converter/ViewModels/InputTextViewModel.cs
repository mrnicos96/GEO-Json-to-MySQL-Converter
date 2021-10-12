using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GEO_Json_to_MySQL_Converter.ViewModels
{
    class InputTextViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Question { get; set; }

        protected string inputValue;
        public string InputValue
        {
            get => inputValue;
            set
            {
                inputValue = value;
                OnPropertyChanged();
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
