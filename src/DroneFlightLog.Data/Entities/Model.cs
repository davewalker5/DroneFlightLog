using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Model
    {
        [Key]
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string Name { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
