using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GEO_Json_to_MySQL_Converter.Models
{
    public class Sirialaser
    {
        public static void DesirialaseFile(string file, string pathDB, string tableName, ObservableCollection<LogNode> Log)
        {
            if (CheakFile(file, out string correctFile))
                return;
            ReadData(correctFile, out string data);
            using (var connection = new SqliteConnection(pathDB))
            {
                connection.Open();

                SqliteCommand commandCreate = new SqliteCommand
                {
                    Connection = connection,
                    CommandText = $"CREATE TABLE {tableName}(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Name " +
                    "TEXT NOT NULL, Longitude TEXT NOT NULL, Latitude TEXT NOT NULL)"
                };
                commandCreate.ExecuteNonQuery();

                int Index;
                string name;
                string coordinateString;
                string[] arrayCoordinates;

                while (data.IndexOf("Feature") != -1)
                {
                    Index = data.IndexOf("osm_type");
                    name = data.Substring(Index);
                    Index = name.IndexOf("name");
                    name = name.Substring(Index);
                    Index = name.IndexOf(",");
                    name = name.Substring(0, Index - 1);


                    Index = data.IndexOf("[[[[");
                    coordinateString = data.Substring(Index + 4);
                    Index = coordinateString.IndexOf("]]]]}");
                    coordinateString = coordinateString.Remove(Index);
                    coordinateString = coordinateString.Replace("]]],[[[", "],[");
                    coordinateString = coordinateString.Replace("],[", ",");
                    arrayCoordinates = coordinateString.Split(',');

                    //object lastItem = arrayCoordinates.GetValue(arrayCoordinates.Length-1);

                    foreach (var coordinate in arrayCoordinates)
                    {
                        Index = coordinate.IndexOf(",");
                        SqliteCommand command = new SqliteCommand
                        {
                            Connection = connection,
                            CommandText = $"INSERT INTO {tableName} (Name, Longitude, Latitude) " +
                            $"VALUES ({name}, {coordinate.Substring(0, Index)}, " +
                            $"{coordinate.Substring(Index - 1)})"
                        };
                        int number = command.ExecuteNonQuery();
                    }

                    Index = data.IndexOf("]]]]}");
                    data = data.Substring(Index + 5);
                }

            }
        }
        private static bool CheakFile(string file, out string correctFile)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.Exists)
            {
                int index = file.IndexOf(".geojson");
                if (index != 0)
                {
                    correctFile = file.Remove(index);
                    fileInfo.CopyTo($"{correctFile}.json", true);
                }
                else
                {
                    correctFile = file;
                }
                return true;
            }
            else
            {
                correctFile = null;
                return false;
            }
        }

        static void ReadData(string file, out string data)
        {
            do
            {
                Task.Run(() => Read(file));
            }
            while (Read(file).Result != null);
            data = Read(file).Result;
        }

        static async Task<string> Read(string file)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            var rawData = await JsonSerializer.DeserializeAsync<object>(fs);
            var data = rawData.ToString();
            return data;
        }
    }
}
