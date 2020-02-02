using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Drone
    {
        [Key]
        public int Id { get; set; }
        public int ModelId { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }

        public Model Model { get; set; }
        public IList<Flight> Flights { get; set; }
    }
}
