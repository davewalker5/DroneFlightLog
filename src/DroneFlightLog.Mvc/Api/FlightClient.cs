﻿using System;
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
    public class FlightClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "Flights";

        public FlightClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Retrieve a single flight given its ID
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightAsync(int flightId)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{flightId}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
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
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
            return flight;
        }

        /// <summary>
        /// Update an existing flight
        /// </summary>
        /// <param name="id"></param>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="operatorId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<Flight> UpdateFlightAsync(int id, int droneId, int locationId, int operatorId, DateTime start, DateTime end)
        {
            dynamic template = new
            {
                Id = id,
                LocationId = locationId,
                OperatorId = operatorId,
                DroneId = droneId,
                Start = start,
                End = end
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
            return flight;
        }
    }
}
