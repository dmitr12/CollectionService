using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.Api.NumbersApi
{
    public class NumbersInfo : ApiBase
    {
        public string Text { get; set; }
        public string Number { get; set; }
        public override string ApiName => "NumbersApi";
    }
}
