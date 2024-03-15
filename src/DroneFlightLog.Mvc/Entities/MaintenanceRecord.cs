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

        [DisplayName("Work Date")]
        [Required(ErrorMessage = "You must provide a work date")]
        public DateTime DateCompleted { get; set; }

        [DisplayName("Work Type")]
        [Required(ErrorMessage = "You must select a maintenance work type")]
        public MaintenanceRecordType RecordType { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "You must provide a description")]
        public string Description { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; }

        public Drone Drone { get; set; }
        public Maintainer Maintainer { get; set; }

        /// <summary>
        /// Used during editing to capture a string representation of the date that
        /// is subsequently parsed to a DateTime
        /// </summary>
        [DisplayName("Work Date")]
        [Required(ErrorMessage = "You must provide a work date")]
        public string DateWorkCompleted { get; set; }

        /// <summary>
        /// Return the date completed as a string correctly formatted for the UI's date picker
        /// </summary>
        public string DateCompletedFormatted
        {
            get
            {
                return DateCompleted.ToString("dd-MMM-yyyy");
            }
        }

        /// <summary>
        /// Return the full name of the maintainer
        /// </summary>
        /// <returns></returns>
        public string MaintainerFullName
        {
            get
            {
                return Maintainer != null ? Maintainer.MaintainerFullName : "";
            }
        }
    }
}
