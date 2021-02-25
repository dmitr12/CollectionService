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
            return GetStringCsv(ObjectToDictionary(obj, obj.GetType()));
        }

        private Dictionary<string, string> ObjectToDictionary<T>(T obj, Type t)
        {
            var dict = new Dictionary<string, string>();
            foreach (var v in t.GetProperties())
            {
                if (v.PropertyType.IsPrimitive || v.PropertyType == typeof(string))
                    dict.Add(v.Name, t.GetProperty($"{v.Name}").GetValue(obj).ToString());
                else
                {
                    var returnedDict = ObjectToDictionary(t.GetProperty($"{v.Name}").GetValue(obj), v.PropertyType);
                    foreach (var item in returnedDict)
                        dict.Add(item.Key, item.Value);
                }
            }
            return dict;
        }

        private StringBuilder GetStringCsv(Dictionary<string,string> valuePairs)
        {
            var sb = new StringBuilder();
            var heads = new StringBuilder();
            var values = new StringBuilder();
            char seporator = ',';
            foreach (var item in valuePairs.Reverse())
            {
                heads.Append($"{item.Key}{seporator}");
                values.Append($"\"{item.Value}\"{seporator}");
            }
            sb.AppendLine(heads.Remove(heads.Length - 1, 1).ToString());
            sb.AppendLine(values.Remove(values.Length - 1, 1).ToString());
            return sb;
        }
    }
}
