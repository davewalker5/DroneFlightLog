using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        public string Description { get; set; }
        public string Notes { get; set; }

        public Maintainer Maintainer { get; set; }
    }
}
