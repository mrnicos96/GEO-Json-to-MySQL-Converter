using System.Windows;

namespace GEO_Json_to_MySQL_Converter.Views
{
    public partial class InputTextWindow : Window
    {
        public InputTextWindow()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
