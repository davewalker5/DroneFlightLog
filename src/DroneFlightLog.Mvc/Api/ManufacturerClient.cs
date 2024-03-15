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
    public class ManufacturerClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Manufacturers";
        private const string CacheKey = "Manufacturers";

        public ManufacturerClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of manufacturers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Manufacturer>> GetManufacturersAsync()
        {
            List<Manufacturer> manufacturers = _cache.Get<List<Manufacturer>>(CacheKey);
            if (manufacturers == null)
            {
                string json = await SendIndirectAsync(RouteKey, null, HttpMethod.Get);
                manufacturers = JsonConvert.DeserializeObject<List<Manufacturer>>(json);
                _cache.Set(CacheKey, manufacturers, _settings.Value.CacheLifetimeSeconds);
            }
            return manufacturers;
        }

        /// <summary>
        /// Return the manufacturer with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Manufacturer> GetManufacturerAsync(int id)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json);
            return manufacturer;
        }

        /// <summary>
        /// Create a new manufacturer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> AddManufacturerAsync(string name)
        {
            _cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json);
            return manufacturer;
        }

        /// <summary>
        /// Update an existing manufacturer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> UpdateManufacturerAsync(int id, string name)
        {
            _cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json);
            return manufacturer;
        }
    }
}
