using GEO_Json_to_MySQL_Converter.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;

namespace GEO_Json_to_MySQL_Converter.Utils
{
    class Sqlite
    {
        public static void WriteDataToDB(string data, string pathDB, string tableName, ObservableCollection<LogNode> Log)
        {
            GetQteStrings(data, Log, out int qteStrings);

            using (var connection = new SqliteConnection($@"Data Source={pathDB}"))
            {
                connection.Open();
                if (!TableNameExist(pathDB, tableName))
                {
                    SqliteCommand commandCreate = new SqliteCommand
                    {
                        Connection = connection,
                        CommandText = $"CREATE TABLE {tableName}(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Name " +
                    "TEXT NOT NULL, Longitude TEXT NOT NULL, Latitude TEXT NOT NULL)"
                    };
                    commandCreate.ExecuteNonQuery();
                }


                int Index;
                string name;
                string coordinateString;
                string[] arrayCoordinates;
                int stringNomber = 0;
                var mesage = new LogNode();
                int coordinateNomber = 0;

                while (data.IndexOf("Feature") != -1)
                {
                    stringNomber++;

                    Index = data.IndexOf("osm_type");
                    name = data.Substring(Index);
                    Index = name.IndexOf("name");
                    name = name.Substring(Index + 7);
                    Index = name.IndexOf(",");
                    name = name.Substring(0, Index - 1);


                    Index = data.IndexOf("[[[[");
                    coordinateString = data.Substring(Index + 4);
                    Index = coordinateString.IndexOf("]]]]}");
                    coordinateString = coordinateString.Remove(Index);
                    coordinateString = coordinateString.Replace("]]],[[[", "],[");
                    coordinateString = coordinateString.Replace("],[", "+");
                    arrayCoordinates = coordinateString.Split('+');

                    //object lastItem = arrayCoordinates.GetValue(arrayCoordinates.Length-1);

                    mesage = new LogNode();
                    mesage.Date_time = DateTime.Now.ToString();
                    mesage.Status = $"Количество кординат в строке {stringNomber}, равно {arrayCoordinates.Length}";
                    Log.Insert(0, mesage);

                    coordinateNomber = 0;

                    foreach (var coordinate in arrayCoordinates)
                    {
                        coordinateNomber++;

                        Index = coordinate.IndexOf(",");
                        SqliteCommand command = new SqliteCommand
                        {
                            Connection = connection,
                            CommandText = $"INSERT INTO {tableName} (Name, Longitude, Latitude) " +
                            $"VALUES ('{name}', '{coordinate.Substring(0, Index)}', " +
                            $"'{coordinate.Substring(Index - 1)}')"
                        };
                        int number = command.ExecuteNonQuery();

                        mesage = new LogNode();
                        mesage.Date_time = DateTime.Now.ToString();
                        mesage.Status = $"Добавлена в БД кордината номер {coordinateNomber} из {arrayCoordinates.Length}";
                        Log.Insert(0, mesage);
                    }

                    Index = data.IndexOf("]]]]}");
                    data = data.Substring(Index + 5);

                    mesage = new LogNode();
                    mesage.Date_time = DateTime.Now.ToString();
                    mesage.Status = $"Обработана строка номер {stringNomber} из {qteStrings}";
                    Log.Insert(0, mesage);
                    connection.Close();
                }
            }
        }

        public static bool TableNameExist(string pathDB, string tableName)
        {
            using (var connection = new SqliteConnection($@"Data Source={pathDB}"))
            {
                SqliteCommand command = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection);
                connection.Open();
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            var name = reader.GetValue(0);
                            if (name.ToString() == tableName)
                            {
                                connection.Close();
                                return true;

                            }    
                                
                        }
                        connection.Close();
                        return false;

                    }
                    else
                    {
                        connection.Close();
                        return false;
                    }
                        
                       
                }
            }
        }

        static void GetQteStrings(string data, ObservableCollection<LogNode> Log, out int qteStrings)
        {
            qteStrings = 0;
            while (data.IndexOf("Feature") != -1)
            {
                qteStrings++;
                data = data.Substring(data.IndexOf("]]]]}") + 5);
            }
            var mesage = new LogNode();
            mesage.Date_time = DateTime.Now.ToString();
            mesage.Status = $"Количество строк в файле равно {qteStrings}";
            Log.Insert(0, mesage);
        }
    }

}
