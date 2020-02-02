using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Operator
    {
        [Key]
        public int Id { get; set; }
        public int AddressId { get; set; }
        public string FirstNames { get; set; }
        public string Surname { get; set; }
        public string OperatorNumber { get; set; }
        public string FlyerNumber { get; set; }
        public DateTime DoB { get; set; }

        public Address Address { get; set; }
    }
}
