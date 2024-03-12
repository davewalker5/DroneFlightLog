using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    }
}
