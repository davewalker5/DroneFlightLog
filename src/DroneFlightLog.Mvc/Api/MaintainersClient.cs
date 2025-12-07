using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DroneFlightLog.Mvc.Api
{
    public class MaintainersClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Maintainers";
        private const string CacheKey = "Maintainers";

        public MaintainersClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of maintainer details
        /// </summary>
        /// <returns></returns>
        public async Task<List<Maintainer>> GetMaintainersAsync()
        {
            List<Maintainer> maintainers = _cache.Get<List<Maintainer>>(CacheKey);
            if (maintainers == null)
            {
                string json = await SendIndirectAsync(RouteKey, null, HttpMethod.Get);
                maintainers = JsonConvert.DeserializeObject<List<Maintainer>>(json);
                _cache.Set(CacheKey, maintainers, _settings.Value.CacheLifetimeSeconds);
            }
            return maintainers;
        }

        /// <summary>
        /// Return the maintainer with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Maintainer> GetMaintainerAsync(int id)
        {
            List<Maintainer> maintainers = await GetMaintainersAsync();
            Maintainer maintainer = maintainers.First(l => l.Id == id);
            return maintainer;
        }

        /// <summary>
        /// Create a new maintainer
        /// </summary>
        /// <param name="firstNames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public async Task<Maintainer> AddMaintainerAsync(string firstNames, string surname)
        {
            _cache.Remove(CacheKey);

            dynamic template = new
            {
                FirstNames = firstNames,
                Surname = surname
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Maintainer maintainer = JsonConvert.DeserializeObject<Maintainer>(json);
            return maintainer;
        }

        /// <summary>
        /// Update an existing maintainer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstNames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public async Task<Maintainer> UpdateMaintainerAsync(int id, string firstNames, string surname)
        {
            _cache.Remove(CacheKey);

            dynamic template = new
            {
                Id = id,
                FirstNames = firstNames,
                Surname = surname
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Maintainer maintainer = JsonConvert.DeserializeObject<Maintainer>(json);
            return maintainer;
        }
    }
}
