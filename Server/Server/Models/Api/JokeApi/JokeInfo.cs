using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Api.JokeApi
{
    public class JokeInfo: ApiBase
    {
        public string Joke { get; set; }
        public string Category { get; set; }
        public override string ApiName => "JokeApi";
    }
}
