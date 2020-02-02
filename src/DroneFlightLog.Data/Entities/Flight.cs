using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        public int DroneId { get; set; }
        public int LocationId { get; set; }
        public int OperatorId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Drone Drone { get; set; }
        public Location Location { get; set; }
        public Operator Operator { get; set; }
        public IList<FlightPropertyValue> Properties { get; set; }
    }
}
