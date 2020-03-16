using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Data.Entities
{
    public class FlightLogRole
    {
        [Key]
        public int Id { get; set; }
        public string Role { get; set; }
    }
}
