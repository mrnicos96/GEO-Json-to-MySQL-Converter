using System;
using System.Windows;
using GEO_Json_to_MySQL_Converter.ViewModels;
using GEO_Json_to_MySQL_Converter.Views;

namespace GEO_Json_to_MySQL_Converter.Utils
{
    public class RequestWindows
    {
        public static void  RequestFile(out string fileName)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { RestoreDirectory = true };
            bool? result = dialog.ShowDialog();
            if (result == true)
                fileName = dialog.FileName;
            else
                fileName = String.Empty;
            dialog = null;
            result = null;
        }

        public static void RequestQuestion(string text, out bool isNewDB)
        {
            isNewDB = MessageBox.Show(text, "GEO Json to MySQL Converter", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes;
        }

        public static bool RequestOpenNewDB(string title, string type, out string fileName)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Title = title, Filter = type };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
                fileName = dialog.FileName;
            else
                fileName = String.Empty;
            return result.Value;
        }

        public static bool RequestOpenDB(string filter, out string fileName)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = filter, FilterIndex = 2, RestoreDirectory = true };
            bool? result = dialog.ShowDialog();
            if (result == true)
                fileName = dialog.FileName;
            else
                fileName = String.Empty;
            return result.Value;
        }

        public static Tuple<bool, string> RequestInputText(string title, string question, string defaultText)
        {
            var model = new InputTextViewModel
            {
                Title = title,
                Question = question,
                InputValue = defaultText
            };

            var result = new InputTextWindow
            {
                DataContext = model,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }.ShowDialog();

            return Tuple.Create(result ?? false, model.InputValue);
        }
    }
}
