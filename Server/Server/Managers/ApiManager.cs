using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using Server.DI;
using Server.Interfaces;
using Server.Models.Api;
using Server.Models.Api.JokeApi;
using Server.Models.Api.NumbersApi;
using Server.Models.Api.WeatherApi;
using Server.Models.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Managers
{
    public class ApiManager<T>: IApiManager<T> where T : ApiBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ApiRepository apiRepository;
        private readonly int apiId;
        private readonly IConfiguration config;

        public ApiManager(ApiRepository apiRepository, int apiId, IConfiguration config)
        {
            this.apiRepository = apiRepository;
            this.apiId = apiId;
            this.config = config;
        }

        public Api GetApiInfo()
        {
            return apiRepository.GetItemById(apiId).Result;
        }

        public T GetFilteredData(string queryString, string filterColumn, string filterParameterValue)
        {
            try
            {
                StringBuilder sb = new StringBuilder(queryString);
                if (filterColumn == "/")
                {
                    if (queryString.Contains('?'))
                    {
                        int index = queryString.IndexOf('?');
                        sb.Insert(index, $"{filterColumn}{filterParameterValue}");
                    }
                    else
                        sb.Append(filterParameterValue);
                }
                else
                    sb.Append($"&{filterColumn}={filterParameterValue}");
                var client = new RestClient(sb.ToString());
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", config.GetSection("RequestHeaders").GetSection("Content-Type").Value);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return null;
        }
    }
}
