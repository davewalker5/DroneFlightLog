using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Maintainer
    {
        [Key]
        public int Id { get; set; }
        public string FirstNames { get; set; }
        public string Surname { get; set; }
    }
}
