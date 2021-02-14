using Microsoft.Data.Sqlite;
using Server.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IDbHelper
    {
        Task<int> ExecuteQuery<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class;
        Task<List<T>> GetData<T>(string queryString, T objForParameters, List<string> parameterNames) where T: class;
        Task<bool> HasRows<T>(string queryString, T objForParameters, List<string> parameterNames) where T : class;
        Task Close();
    }
}
