using GEO_Json_to_MySQL_Converter.Models;
using GEO_Json_to_MySQL_Converter.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace GEO_Json_to_MySQL_Converter.ViewModels
{

    public class MainViewModel : BusyViewModel
    {
        protected Sqlite sqlite = new Sqlite();
        public RelayCommand openFileCommand;
        protected string log = String.Empty;
        public string Log
        {
            get => log;
            protected set
            {
                log = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel() => Sqlite.NewProgressStatus += AddLog;

        public RelayCommand OpenFileCommand => openFileCommand ??
                  (openFileCommand = new RelayCommand((o) =>
                  {
                      try
                      {
                          OnBusy("Waiting ...");
                          RequestWindows.RequestFile(out string file);
                          RequestWindows.RequestQuestion("Создать новую базу данных для сохранения даных ?", out bool isNewDB);
                          string pathDB;
                          if (isNewDB)
                          {
                              RequestWindows.RequestOpenNewDB("Выбор новой БД", "db files (*.db)|*.db", out pathDB);
                          }
                          else
                          {
                              RequestWindows.RequestOpenDB("db files (*.db)|*.db", out pathDB);
                          }

                          string tableName = RequestWindows.RequestTableName();
                          string data = Sirialaser.DesirialaseFile(file);
                          Task.Run(() => AsyncWriteDataToDB(data, pathDB, tableName));
                          Sqlite.WriteDataToDB(data, pathDB, tableName);
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

        private void AddLog(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    if (Log.Length > 0)
                        log += "\n";
                    Log += $"{ DateTime.Now.ToString("HH:mm:ss ") } { message }";
                }
            );
        }
        private async Task AsyncWriteDataToDB(string data, string pathDB, string tableName)
        {
            await Task.Run(() =>
            {
                Sqlite.WriteDataToDB(data,
                                     pathDB,
                                     tableName);
            });
        }
    }
}
