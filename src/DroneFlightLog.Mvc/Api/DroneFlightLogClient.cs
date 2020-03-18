using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Controllers;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DroneFlightLog.Mvc.Api
{
    public class DroneFlightLogClient
    {
        private const string CacheManufacturers = "Manufacturers";
        private const string CacheModels = "Models";
        private const string CacheDrones = "Drones";
        private const string CacheLocations = "Locations";
        private const string CacheOperators = "Operators";
        private const string CacheFlightProperties = "FlightProperties";

        private readonly HttpClient _client;
        private readonly IOptions<AppSettings> _settings;
        private readonly ICacheWrapper _cache;

        public DroneFlightLogClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
        {
            _cache = cache;

            // Configure the Http client
            _settings = settings;
            _client = client;
            _client.BaseAddress = new Uri(_settings.Value.ApiUrl);

            // Retrieve the token from session and create the authentication
            // header, if present
            string token = accessor.HttpContext.Session.GetString(LoginController.TokenSessionKey);
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthenticationHeader(token);
            }
        }

        /// <summary>
        /// Retrieve a single flight given its ID
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightAsync(int flightId)
        {
            // TODO : This needs to be replaced with a call to retrieve a single flight
            // by Id. For now, retrieve an arbitrary large number that will cover them
            // all then pick the one required
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == "Flights").Route;
            string route = $"{baseRoute}/1/1000000";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Flight> flights = JsonConvert.DeserializeObject<List<Flight>>(json);
            Flight flight = flights.First(f => f.Id == flightId);
            return flight;
        }

        /// <summary>
        /// Create a new flight
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="operatorId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<Flight> AddFlightAsync(int droneId, int locationId, int operatorId, DateTime start, DateTime end)
        {
            dynamic template = new
            {
                LocationId = locationId,
                OperatorId = operatorId,
                DroneId = droneId,
                Start = start,
                End = end
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync("Flights", data, HttpMethod.Post);

            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
            return flight;
        }

        /// <summary>
        /// Add the authorization header to the default request headers
        /// </summary>
        /// <param name="token"></param>
        private void SetAuthenticationHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Given a route name, some data (null in the case of GET) and an HTTP method,
        /// look up the route from the application settings then send the request to
        /// the service and return the JSON response
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private async Task<string> SendIndirectAsync(string routeName, string data, HttpMethod method)
        {
            string route = _settings.Value.ApiRoutes.First(r => r.Name == routeName).Route;
            string json = await SendDirectAsync(route, data, method);
            return json;
        }

        /// <summary>
        /// Given a route, some data (null in the case of GET) and an HTTP method,
        /// send the request to the service and return the JSON response
        /// </summary>
        /// <param name="route"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private async Task<string> SendDirectAsync(string route, string data, HttpMethod method)
        {
            string json = null;

            HttpResponseMessage response = null;
            if (method == HttpMethod.Get)
            {
                response = await _client.GetAsync(route);
            }
            else if (method == HttpMethod.Post)
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await _client.PostAsync(route, content);
            }
            else if (method == HttpMethod.Put)
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await _client.PutAsync(route, content);
            }

            if ((response != null) && response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }

            return json;
        }
    }
}
