using DroneFlightLog.Data.Binders;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class MaintenanceRecord
    {
        [Key]
        public int Id { get; set;  }
        public int MaintainerId { get; set; }
        public int DroneId { get; set; }
        public DateTime DateCompleted { get; set; }

        [JsonConverter(typeof(MaintenanceRecordTypeJsonConverter))]
        public MaintenanceRecordType RecordType { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public Maintainer Maintainer { get; set; }
        public Drone Drone { get; set; }
    }
}
