using Server.Interfaces;
using Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Api.WeatherApi
{
    public class WeatherInfo: ApiBase
    {
        public Main Main { get; set; } = new Main();
        public Wind Wind { get; set; } = new Wind();
        public string Name { get; set; }
        public override string ApiName => "WeatherApi";
    }
}
