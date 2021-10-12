using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace GEO_Json_to_MySQL_Converter.Models
{
    public class LogNode : INotifyPropertyChanged
    {
        private string date_time;
        private string status;

        public string Date_time
        {
            get { return date_time; }
            set
            {
                date_time = value;
                OnPropertyChanged("Date_time");
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
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

