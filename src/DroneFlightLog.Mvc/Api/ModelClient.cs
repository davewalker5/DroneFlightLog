using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DroneFlightLog.Mvc.Api
{
    public class ModelClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Models";
        private const string CacheKey = "Models";

        public ModelClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of drone models
        /// </summary>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync()
        {
            List<Model> models = _cache.Get<List<Model>>(CacheKey);
            if (models == null)
            {
                string json = await SendIndirectAsync(RouteKey, null, HttpMethod.Get);
                models = JsonConvert.DeserializeObject<List<Model>>(json);
                _cache.Set(CacheKey, models, _settings.Value.CacheLifetimeSeconds);
            }
            return models;
        }

        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddModelAsync(string name, int manufacturerId)
        {
            _cache.Remove(CacheKey);

            dynamic template = new
            {
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Model model = JsonConvert.DeserializeObject<Model>(json);
            return model;
        }
    }
}
