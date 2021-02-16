using Newtonsoft.Json;
using RestSharp;
using Server.Interfaces;
using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Managers
{
    public class TaskManager
    {
        private readonly IDbHelper dbHelper;

        public TaskManager(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public List<Api> GetListApi()
        {
            try
            {
                return dbHelper.GetData<Api>("select * from apies", null, null).Result;
            }
            finally
            {
                dbHelper.Close();
            }
        }

        public T GetFilteredData<T>(string queryString, IDictionary<string, string> queryParameters) where T: class
        {
            T data = null;
            StringBuilder sb = new StringBuilder(queryString);
            sb.Append("?");
            foreach (KeyValuePair<string, string> keyValue in queryParameters)
                sb.Append($"{keyValue.Key}={keyValue.Value}&");
            sb.Remove(sb.Length - 1, 1);
            var client = new RestClient(sb.ToString());
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
                data = JsonConvert.DeserializeObject<T>(response.Content);
            return data;
        }
    }
}
