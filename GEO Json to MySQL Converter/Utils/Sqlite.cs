using Microsoft.Data.Sqlite;
using System;
using System.Threading;

namespace GEO_Json_to_MySQL_Converter.Utils
{
    public class Sqlite
    {
        public delegate void ProgressStatus(string status);
        public event ProgressStatus NewProgressStatus;
        public void WriteDataToDB(string data, string pathDB, string tableName)
        {
            GetQteStrings(data, out int qteStrings);
            NewProgressStatus.Invoke($"Количество строк в файле равно {qteStrings}");
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

                NewProgressStatus.Invoke($"3");
                int Index;
                string name;
                string coordinateString;
                string[] arrayCoordinates;
                int stringNomber = 0;

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

                    NewProgressStatus.Invoke($"Количество кординат в строке {stringNomber}, равно {arrayCoordinates.Length}. " +
                        $"Расчетное время обработки строки {stringNomber} составит {Math.Ceiling(((arrayCoordinates.Length) * 0.0080358447377392) / 60)} мин");
                    Thread.Sleep(100);

                    foreach (var coordinate in arrayCoordinates)
                    {
                        Index = coordinate.IndexOf(",");
                        SqliteCommand command = new SqliteCommand
                        {
                            Connection = connection,
                            CommandText = $"INSERT INTO {tableName} (Name, Longitude, Latitude) " +
                            $"VALUES ('{name}', '{coordinate.Substring(Index + 1)}', " +
                            $"'{coordinate.Substring(0, Index)}')"
                        };
                        int number = command.ExecuteNonQuery();
                    }

                    Index = data.IndexOf("]]]]}");
                    data = data.Substring(Index + 5);

                    NewProgressStatus.Invoke($"Обработана строка номер {stringNomber} из {qteStrings}");
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

        static void GetQteStrings(string data, out int qteStrings)
        {
            qteStrings = 0;
            while (data.IndexOf("Feature") != -1)
            {
                qteStrings++;
                data = data.Substring(data.IndexOf("]]]]}") + 5);
            }
        }
    }

}
