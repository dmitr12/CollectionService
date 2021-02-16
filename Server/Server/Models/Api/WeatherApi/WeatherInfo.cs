using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Api.WeatherApi
{
    public class WeatherInfo
    {
        public string Name { get; set; }
        public Main Main { get; set; }
        public Wind Wind { get; set; }
    }
}
