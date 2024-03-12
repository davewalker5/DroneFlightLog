using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DroneFlightLog.Mvc.Api
{
    public class MaintenanceRecordClient : DroneFlightLogClientBase
    {
        private const string RouteKey = "MaintenanceRecords";
        private const string CacheKey = "MaintenanceRecords";

        public MaintenanceRecordClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Retrieve a single maintenance record given its ID
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public async Task<MaintenanceRecord> GetMaintenanceRecordAsync(int recordId)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/{recordId}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            MaintenanceRecord maintenanceRecord = JsonConvert.DeserializeObject<MaintenanceRecord>(json);
            return maintenanceRecord;
        }

        /// <summary>
        /// Retrieve all maintenance records for a drone
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public async Task<List<MaintenanceRecord>> GetMaintenanceRecordsForDoneAsync(int droneId)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/drone/{droneId}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<MaintenanceRecord> maintenanceRecords = JsonConvert.DeserializeObject<List<MaintenanceRecord>>(json);
            return maintenanceRecords;
        }

        /// <summary>
        /// Create a new maintenance record
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<MaintenanceRecord> AddMaintenanceRecordAsync(int maintainerId, int droneId, DateTime date, string description, string notes)
        {
            dynamic template = new
            {
                MaintainerId = maintainerId,
                DroneId = droneId,
                DateCompleted = date,
                Description = description,
                Notes = notes
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            MaintenanceRecord maintenanceRecord = JsonConvert.DeserializeObject<MaintenanceRecord>(json);
            return maintenanceRecord;
        }

        /// <summary>
        /// Update an existing maintenance record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<MaintenanceRecord> UpdateMaintenanceRecordAsync(int id, int maintainerId, int droneId, DateTime date, string description, string notes)
        {
            dynamic template = new
            {
                Id = id,
                MaintainerId = maintainerId,
                DroneId = droneId,
                DateCompleted = date,
                Description = description,
                Notes = notes
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            MaintenanceRecord maintenanceRecord = JsonConvert.DeserializeObject<MaintenanceRecord>(json);
            return maintenanceRecord;
        }
    }
}
