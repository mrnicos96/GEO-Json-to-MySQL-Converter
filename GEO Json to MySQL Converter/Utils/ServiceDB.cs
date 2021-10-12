using System;
using Microsoft.Data.Sqlite;

namespace GEO_Json_to_MySQL_Converter.Utils
{
    public class ServiceDB
    {
        public static void ConectToDB(string path)
        {
            using (var connection = new SqliteConnection(path))
            {
                connection.Open();
            }
        }
    }
}
