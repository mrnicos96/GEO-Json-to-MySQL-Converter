using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GEO_Json_to_MySQL_Converter.Utils;
using GEO_Json_to_MySQL_Converter.Models;

namespace GEO_Json_to_MySQL_Converter.ViewModels
{
    
    public class MainViewModel : BusyViewModel
    {
        private RelayCommand openFileCommand;
        public ObservableCollection<LogNode> Log { get; set; }

        public MainViewModel()
        {
            Log = new ObservableCollection<LogNode>();
        }

        public RelayCommand OpenFileCommand => openFileCommand ??
                  (openFileCommand = new RelayCommand((o) =>
                  {
                      try
                      {
                          OnBusy("Waiting ...");
                          RequestWindows.RequestFile(out string file);
                          RequestWindows.RequestQuestion("Создать новую базу данных ?", out bool isNewDB);
                          string pathDB;
                          if (isNewDB)
                          {
                              RequestWindows.RequestOpenNewDB("Выбор новой БД", "db files (*.db)|*.db", out pathDB);
                          }
                          else
                          {
                              RequestWindows.RequestOpenDB("db files (*.db)|*.db", out pathDB);
                          }
                          RequestWindows.RequestQuestion("Начать считывание файла с первой строки?", out bool isNewAttempt);
                          if (isNewAttempt)
                          {
                              СontinueDesirialaseFile(file, pathDB, Log);
                          }
                          else
                          {
                              
                              DesirialaseFile(file, pathDB, Log);
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
