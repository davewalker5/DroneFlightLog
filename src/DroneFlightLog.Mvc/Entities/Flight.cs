using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class Flight
    {
        #region Entity Properties From The Service
        public int Id { get; set; }

        [DisplayName("Location")]
        [Required(ErrorMessage = "You must select a location")]
        public int LocationId { get; set; }

        [DisplayName("Operator")]
        [Required(ErrorMessage = "You must select an operator")]
        public int OperatorId { get; set; }

        [DisplayName("Drone")]
        [Required(ErrorMessage = "You must select a drone")]
        public int DroneId { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Drone Drone { get; set; }
        public Location Location { get; set; }
        public Operator Operator { get; set; }
        public List<FlightPropertyValue> Properties { get; set; }
        #endregion

        #region Editor Properties
        [DisplayName("Start Date")]
        [Required(ErrorMessage = "You must provide a start date")]
        public string StartDate { get; set; }

        [DisplayName("Start Time")]
        [Required(ErrorMessage = "You must provide a start time")]
        public string StartTime { get; set; }

        [DisplayName("End Date")]
        [Required(ErrorMessage = "You must provide an end date")]
        public string EndDate { get; set; }

        [DisplayName("End Time")]
        [Required(ErrorMessage = "You must provide an end time")]
        public string EndTime { get; set; }
        #endregion

        #region Flight List Presentation Properties
        public string StartTimeFormatted { get { return Start.ToString("HH:mm"); } }
        public string StartDateFormatted { get { return Start.ToString("dd-MMM-yyyy"); } }
        public string EndTimeFormatted { get { return End.ToString("HH:mm"); } }
        public string EndDateFormatted { get { return End.ToString("dd-MMM-yyyy"); } }
        public int Duration { get { return (int)(End - Start).TotalMinutes; } }

        public string GetOperatorName()
        {
            string name = "";

            if (Operator != null)
            {
                name = $"{Operator.FirstNames} {Operator.Surname}";
            }

            return name;
        }
        #endregion
    }
}
