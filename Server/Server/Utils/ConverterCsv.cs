using Server.Models.Api.WeatherApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
    public class ConverterCsv
    {
        public StringBuilder ConvertToCsv<T>(T obj)
        {
            if (typeof(T) == typeof(WeatherInfo))
            {
                WeatherInfo wi = obj as WeatherInfo;
                return GetStringCsv(new WeatherCSV { Name = wi.Name, Humidity = wi.Main.Humidity, Temp = wi.Main.Temp, WindSpeed = wi.Wind.Speed },
                    new List<string> { "Name", "Humidity", "Temp", "WindSpeed" });
            }
            return null;
        }

        private StringBuilder GetStringCsv<T>(T obj, List<string> properties)
        {
            Type type = typeof(T);
            StringBuilder sb = new StringBuilder();
            StringBuilder heads = new StringBuilder();
            StringBuilder values = new StringBuilder();
            foreach (string prop in properties)
            {
                heads.Append($"{prop},");
                values.Append($"{type.GetProperty(prop).GetValue(obj)},");
            }
            sb.AppendLine(heads.Remove(heads.Length - 1, 1).ToString());
            sb.AppendLine(values.Remove(values.Length - 1, 1).ToString());
            return sb;
        }
    }
}
