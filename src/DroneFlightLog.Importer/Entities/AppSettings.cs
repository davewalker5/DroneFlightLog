using System;
namespace DroneFlightLog.Importer.Entities
{
    public class AppSettings
    {
        public string DroneColumnName { get; set; }
        public string LocationColumnName { get; set; }
        public string OperatorColumnName { get; set; }
        public string StartDateColumnName { get; set; }
        public string StartTimeColumnName { get; set; }
        public string EndDateColumnName { get; set; }
        public string EndTimeColumnName { get; set; }
    }
}
