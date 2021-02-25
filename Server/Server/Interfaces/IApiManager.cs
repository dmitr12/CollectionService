using Server.Models.Api;
using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IApiManager<out T> where T : ApiBase
    {
        T GetFilteredData(string queryString, string filterColumn, string filterParameterValue);
        Api GetApiInfo();
    }
}
