using Microsoft.Extensions.Configuration;
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
using System.Threading.Tasks;

namespace Server.Managers
{
    public enum ApiesId
    {
        ApiWeather = 2,
        ApiNumber = 3,
        ApiJoke = 4
    }

    public class BaseApiManager
    {
        private readonly ApiRepository apiRepository;
        private readonly IConfiguration config;

        public BaseApiManager(ApiRepository apiRepository, IConfiguration config)
        {
            this.apiRepository = apiRepository;
            this.config = config;
        }

        public IApiManager<ApiBase> GetApiManager(int apiId)
        {
            switch (apiId)
            {
                case (int)ApiesId.ApiWeather: return new ApiManager<WeatherInfo>(apiRepository, apiId, config);
                case (int)ApiesId.ApiNumber: return new ApiManager<NumbersInfo>(apiRepository, apiId, config);
                case (int)ApiesId.ApiJoke: return new ApiManager<JokeInfo>(apiRepository, apiId, config);
            }
            return null;
        }

        public List<Api> GetListApi()
        {
            return apiRepository.GetAllItems().Result.ToList();
        }

    }
}
