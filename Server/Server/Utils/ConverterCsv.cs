using Server.Models.Api.JokeApi;
using Server.Models.Api.NumbersApi;
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
            if (obj.GetType().Name == "WeatherInfo")
            {
                if (obj is WeatherInfo wi)
                    return GetStringCsv(new WeatherCSV { Name = wi.Name, Humidity = wi.Main.Humidity, Temp = wi.Main.Temp, WindSpeed = wi.Wind.Speed },
                    new List<string> { "Name", "Humidity", "Temp", "WindSpeed" });
            }
            else if (obj.GetType().Name == "NumbersInfo")
            {
                if (obj is NumbersInfo ni)
                    return GetStringCsv(new NumbersInfo { Number = ni.Number, Text = ni.Text }, new List<string> { "Number", "Text" });
            }
            else if (obj.GetType().Name == "JokeInfo")
            {
                if (obj is JokeInfo ji)
                    return GetStringCsv(new JokeInfo { Category = ji.Category, Joke = ji.Joke }, new List<string> { "Category", "Joke" });
            }
            return null;
        }

        private StringBuilder GetStringCsv<T>(T obj, List<string> properties)
        {
            var type = typeof(T);
            var sb = new StringBuilder();
            var heads = new StringBuilder();
            var values = new StringBuilder();
            char seporator = ',';
            foreach (string prop in properties)
            {
                heads.Append($"{prop}{seporator}");
                values.Append($"\"{type.GetProperty(prop)?.GetValue(obj)}\"{seporator}");
            }
            sb.AppendLine(heads.Remove(heads.Length - 1, 1).ToString());
            sb.AppendLine(values.Remove(values.Length - 1, 1).ToString());
            return sb;
        }
    }
}
