using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace GEO_Json_to_MySQL_Converter.Utils
{
    public class Sirialaser
    {
        public static string DesirialaseFile(string file)
        {
            return ReadData(file);
        }

        private static string ReadData(string file)
        {
            var acyncRead = Task.Run(() => AsyncReadData(file));
            acyncRead.Wait();
            return acyncRead.Result;
        }

        static async Task<string> AsyncReadData(string file)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            var rawData = await JsonSerializer.DeserializeAsync<object>(fs);
            var data = rawData.ToString();
            return data;
        }
    }
}
