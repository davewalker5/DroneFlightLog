using System;
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
    public class FlightPropertyClient : DroneFlightLogClientBase
    {
        private const string PropertiesRouteKey = "FlightProperties";
        private const string ValuesRouteKey = "FlightPropertyValues";
        private const string CacheKey = "FlightProperties";

        public FlightPropertyClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of available flight properties
        /// </summary>
        /// <returns></returns>
        public async Task<List<FlightProperty>> GetFlightPropertiesAsync()
        {
            List<FlightProperty> properties = _cache.Get<List<FlightProperty>>(CacheKey);
            if (properties == null)
            {
                string json = await SendIndirectAsync(PropertiesRouteKey, null, HttpMethod.Get);
                properties = JsonConvert.DeserializeObject<List<FlightProperty>>(json);
                _cache.Set(CacheKey, properties, _settings.Value.CacheLifetimeSeconds);
            }
            return properties;
        }

        /// <summary>
        /// Return the flight property with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlightProperty> GetFlightPropertyAsync(int id)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == PropertiesRouteKey).Route;
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            FlightProperty property = JsonConvert.DeserializeObject<FlightProperty>(json);
            return property;
        }

        /// <summary>
        /// Create a new flight property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        /// <param name="isSingleInstance"></param>
        /// <returns></returns>
        public async Task<FlightProperty> AddFlightPropertyAsync(string name, int dataType, bool isSingleInstance)
        {
            _cache.Remove(CacheKey);

            dynamic template = new
            {
                Name = name,
                DataType = dataType,
                IsSingleInstance = isSingleInstance
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(PropertiesRouteKey, data, HttpMethod.Post);

            FlightProperty property = JsonConvert.DeserializeObject<FlightProperty>(json);
            return property;
        }

        /// <summary>
        /// Update an existing flight property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        /// <param name="isSingleInstance"></param>
        /// <returns></returns>
        public async Task<FlightProperty> UpdateFlightPropertyAsync(int id, string name)
        {
            _cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == PropertiesRouteKey).Route;
            string route = $"{baseRoute}/{id}";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);

            FlightProperty property = JsonConvert.DeserializeObject<FlightProperty>(json);
            return property;
        }

        /// <summary>
        /// Return a list of flight property values for a specified flight
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<List<FlightPropertyValue>> GetFlightPropertyValuesAsync(int flightId)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == ValuesRouteKey).Route;
            string route = $"{baseRoute}/{flightId}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<FlightPropertyValue> values = JsonConvert.DeserializeObject<List<FlightPropertyValue>>(json);
            return values;
        }

        /// <summary>
        /// Create a new numeric property value
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> AddFlightPropertyNumberValueAsync(int flightId, int propertyId, decimal? propertyValue)
        {
            dynamic template = new
            {
                FlightId = flightId,
                PropertyId = propertyId,
                NumberValue = propertyValue
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(ValuesRouteKey, data, HttpMethod.Post);

            FlightPropertyValue value = JsonConvert.DeserializeObject<FlightPropertyValue>(json);
            return value;
        }

        /// <summary>
        /// Create a new string property value
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> AddFlightPropertyStringValueAsync(int flightId, int propertyId, string propertyValue)
        {
            dynamic template = new
            {
                FlightId = flightId,
                PropertyId = propertyId,
                StringValue = propertyValue
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(ValuesRouteKey, data, HttpMethod.Post);

            FlightPropertyValue value = JsonConvert.DeserializeObject<FlightPropertyValue>(json);
            return value;
        }

        /// <summary>
        /// Update an existing property value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="numberValue"></param>
        /// <param name="stringValue"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> UpdateFlightPropertyValueAsync(int id, decimal? numberValue, string stringValue, DateTime? dateValue)
        {
            dynamic template = new
            {
                Id = id,
                NumberValue = numberValue,
                StringValue = stringValue,
                DateValue = dateValue
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(ValuesRouteKey, data, HttpMethod.Put);

            FlightPropertyValue value = JsonConvert.DeserializeObject<FlightPropertyValue>(json);
            return value;
        }
    }
}
