using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models.DB_Models
{
    public class Api
    {
        public int ApiId { get; set; }
        public string ApiName { get; set; }
        public string BaseUrl { get; set; }
        public string FilterColumn { get; set; }
    }
}
