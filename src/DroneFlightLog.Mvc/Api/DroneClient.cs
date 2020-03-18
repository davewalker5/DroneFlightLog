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
    public class DroneClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Drones";
        private const string CacheKey = "Drones";

        public DroneClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of drones
        /// </summary>
        /// <returns></returns>
        public async Task<List<Drone>> GetDronesAsync()
        {
            List<Drone> drones = _cache.Get<List<Drone>>(CacheKey);
            if (drones == null)
            {
                string json = await SendIndirectAsync(RouteKey, null, HttpMethod.Get);
                drones = JsonConvert.DeserializeObject<List<Drone>>(json);
                _cache.Set(CacheKey, drones, _settings.Value.CacheLifetimeSeconds);
            }
            return drones;
        }

        /// <summary>
        /// Create a new drone
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialNumber"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Drone> AddDroneAsync(string name, string serialNumber, int modelId)
        {
            _cache.Remove(CacheKey);

            dynamic template = new
            {
                Name = name,
                SerialNumber = serialNumber,
                ModelId = modelId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Drone drone = JsonConvert.DeserializeObject<Drone>(json);
            return drone;
        }
    }
}
