using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DroneFlightLog.Mvc.Entities
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }

        [DisplayName("Maintainer")]
        [Required(ErrorMessage = "You must select a maintainer")]
        public int MaintainerId { get; set; }

        [DisplayName("Drone")]
        [Required(ErrorMessage = "You must select a drone")]
        public int DroneId { get; set; }

        [Required(ErrorMessage = "You must select a work date")]
        public DateTime DateCompleted { get; set; }

        [Required(ErrorMessage = "You must select a maintenance type")]
        public MaintenanceRecordType RecordType { get; set; }

        [Required(ErrorMessage = "You must select a description")]
        public string Description { get; set; }

        public string Notes { get; set; }
    }
}
