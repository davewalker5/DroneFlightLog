using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class FlightPropertyValue
    {
        [Key]
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int FlightId { get; set; }

        public string StringValue { get; set; }
        public DateTime? DateValue { get; set; }
        public decimal? NumberValue { get; set; }

        public FlightProperty Property { get; set; }
        public Flight Flight { get; set; }
    }
}
