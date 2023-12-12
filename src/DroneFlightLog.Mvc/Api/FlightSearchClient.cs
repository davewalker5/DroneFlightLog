using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DroneFlightLog.Mvc.Api
{
    public class FlightSearchClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Flights";

        public FlightSearchClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return the specified page of flights
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsAsync(int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByOperatorAsync(int operatorId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/operator/{operatorId}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by drone
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByDroneAsync(int droneId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/drone/{droneId}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by location
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByLocationAsync(int locationId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/location/{locationId}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }

        /// <summary>
        /// Return the specified page of flights, filtered by location
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByDateAsync(DateTime start, DateTime end, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string startDateSegment = HttpUtility.UrlEncode(start.ToString(_settings.Value.ApiDateFormat));
            string endDateSegment = HttpUtility.UrlEncode(end.ToString(_settings.Value.ApiDateFormat));
            string route = $"{baseRoute}/date/{startDateSegment}/{endDateSegment}/{page}/{pageSize}";

            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Flight> flight = JsonConvert.DeserializeObject<List<Flight>>(json);
            return flight;
        }
    }
}
