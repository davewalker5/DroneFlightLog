using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
