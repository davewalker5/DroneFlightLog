using System.Collections.Generic;
using System.Linq;
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
        /// Return the model with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Model> GetModel(int id)
        {
            // TODO : This needs to be replaced with a call to retrieve a single
            // model by Id. For now, retrieve them all then pick the one required
            List<Model> models = await GetModelsAsync();
            Model model = models.First(m => m.Id == id);
            return model;
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

        /// <summary>
        /// Update an existing model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Model> UpdateModelAsync(int id, int manufacturerId, string name)
        {
            _cache.Remove(CacheKey);

            dynamic template = new
            {
                Id = id,
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            Model model = JsonConvert.DeserializeObject<Model>(json);
            return model;
        }
    }
}
