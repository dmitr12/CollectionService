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
            if (typeof(T) == typeof(WeatherInfo))
            {
                WeatherInfo wi = obj as WeatherInfo;
                return GetStringCsv(new WeatherCSV { Name = wi.Name, Humidity = wi.Main.Humidity, Temp = wi.Main.Temp, WindSpeed = wi.Wind.Speed },
                    new List<string> { "Name", "Humidity", "Temp", "WindSpeed" });
            }
            else if(typeof(T)==typeof(NumbersInfo))
            {
                NumbersInfo ni = obj as NumbersInfo;
                return GetStringCsv(new NumbersInfo { Number = ni.Number, Text = ni.Text }, new List<string> { "Number", "Text"});
            }
            else if (typeof(T) == typeof(JokeInfo))
            {
                JokeInfo ji = obj as JokeInfo;
                return GetStringCsv(new JokeInfo { Category = ji.Category, Joke=ji.Joke }, new List<string> { "Category", "Joke" });
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
