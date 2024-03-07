using System.Collections.Generic;

namespace DroneFlightLog.Mvc.Configuration
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ApiUrl { get; set; }
        public List<ApiRoute> ApiRoutes { get; set; }
        public string ApiDateFormat { get; set; }
        public int FlightSearchPageSize { get; set; }
        public int FlightPropertiesPerRow { get; set; }
        public int CacheLifetimeSeconds { get; set; }
    }
}
