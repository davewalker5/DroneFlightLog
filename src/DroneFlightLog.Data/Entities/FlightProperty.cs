using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class FlightProperty
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public FlightPropertyDataType DataType { get; set; }
        public bool IsSingleInstance { get; set; }
    }
}
