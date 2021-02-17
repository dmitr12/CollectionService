using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Api.WeatherApi
{
    public class WeatherCSV
    {
        public string Name { get; set; }
        public float Temp { get; set; }
        public float Humidity { get; set; }
        public float WindSpeed { get; set; }
    }
}
