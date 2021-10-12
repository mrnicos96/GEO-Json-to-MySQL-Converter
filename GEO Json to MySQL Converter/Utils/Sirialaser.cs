using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GEO_Json_to_MySQL_Converter.Utils;

namespace GEO_Json_to_MySQL_Converter.Models
{
    public class Sirialaser
    {
        public void DesirialaseFile(string file, string pathDB, ObservableCollection<LogNode>  Log)
        {
            if (CheakFile(file, out string correctFile))
                return;
            ReadData(correctFile, out string data);
            ServiceDB.ConectToDB(pathDB);

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
                coordinateString = data.Substring(Index+4);
                Index = coordinateString.IndexOf("]]]]}");
                coordinateString = coordinateString.Remove(Index);
                coordinateString = coordinateString.Replace("]]],[[[", "],[");
                coordinateString = coordinateString.Replace("]]],[[[", ",");
                arrayCoordinates = coordinateString.Split(',');

                //object lastItem = arrayCoordinates.GetValue(arrayCoordinates.Length-1);

                foreach (var coordinate in arrayCoordinates)
                {
                    item.Name = name;
                    Index = coordinate.IndexOf(",");
                    item.Longitude = coordinate.Substring(0, Index);
                    item.Latitude = coordinate.Substring(Index - 1);
                    List.Insert(0, item);
                }

                Index = data.IndexOf("]]]]}");
                data = data[(Index + 5)..];

                ListN itemN = new ListN();
                itemN.List = List;
                ListColl.Insert(0, itemN);
            }
        }
        private bool CheakFile (string file, out string correctFile)
        {
            FileInfo fileInfo = new FileInfo(file);
            if(fileInfo.Exists)
            {
                int index = file.IndexOf(".geojson");
                if(index!=0)
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
            data =Read(file).Result;
        }

        static async Task<string>  Read (string file)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            var rawData = await JsonSerializer.DeserializeAsync<object>(fs);
            var data = rawData.ToString();
            return data;
        }
    }
}
