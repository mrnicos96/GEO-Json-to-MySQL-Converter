using GEO_Json_to_MySQL_Converter.ViewModels;
using System.Windows;

namespace GEO_Json_to_MySQL_Converter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
