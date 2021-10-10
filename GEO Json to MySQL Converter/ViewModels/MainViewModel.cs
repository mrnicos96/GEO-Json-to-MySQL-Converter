using System;
using System.Windows;
using GEO_Json_to_MySQL_Converter.Utils;

namespace GEO_Json_to_MySQL_Converter.ViewModels
{
    public class MainViewModel : BusyViewModel
    {
        private RelayCommand openFileCommand;

        public MainViewModel()
        { 

        }

        public RelayCommand OpenFileCommand => openFileCommand ??
                  (openFileCommand = new RelayCommand((o) =>
                  {
                      try
                      {
                          OnBusy("Waiting ...");
                          RequestWindows.RequestFile(out string file);
                          RequestWindows.RequestQuestion("Создать новую базу данных ?", out bool isNewDB);
                          RequestWindows.RequestQuestion("Начать считывание файла с первой строки?", out bool isNewAttempt);
                          if (isNewDB==true)
                          {
                              RequestWindows.RequestNewDB("Выбор БД", "db files (*.db)|*.db", out string pathDB);
                              СontinueDesirialaseFile(file, pathDB, isNewAttempt);
                          }
                          else
                          {
                              RequestWindows.RequestFile(out string pathDB);
                              DesirialaseFile(file, pathDB, isNewAttempt);
                          }
                      }
                      catch (Exception ex)
                      {
                          MessageBox.Show($"{ex.Message} " +
                               "Пришлите мне письмо с описание выших действи вызваших эту ошибку. " +
                              @"bosup@bk.ru" + " Спасибо!",
                              "GEO Json to MySQL Converter - Ошибка!",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                      }
                      finally
                      {

                          OffBusy();
                      }
                  }));
    }
}
