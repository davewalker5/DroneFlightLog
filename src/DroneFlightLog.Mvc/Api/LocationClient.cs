﻿using System.Collections.Generic;
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
    public class LocationClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Locations";
        private const string CacheKey = "Locations";

        public LocationClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
        {
            List<Location> locations = _cache.Get<List<Location>>(CacheKey);
            if (locations == null)
            {
                string json = await SendIndirectAsync(RouteKey, null, HttpMethod.Get);
                locations = JsonConvert.DeserializeObject<List<Location>>(json);
                _cache.Set(CacheKey, locations, _settings.Value.CacheLifetimeSeconds);
            }
            return locations;
        }

        /// <summary>
        /// Return the location with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Location> GetLocationAsync(int id)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Location location = JsonConvert.DeserializeObject<Location>(json);
            return location;
        }

        /// <summary>
        /// Create a new location
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddLocationAsync(string name)
        {
            _cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Location location = JsonConvert.DeserializeObject<Location>(json);
            return location;
        }

        /// <summary>
        /// Update an existing location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> UpdateLocationAsync(int id, string name)
        {
            _cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Location location = JsonConvert.DeserializeObject<Location>(json);
            return location;
        }
    }
}
