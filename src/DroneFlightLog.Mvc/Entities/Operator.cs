using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class Operator
    {
        public int Id { get; set; }
        public int AddressId { get; set; }

        [DisplayName("First Names")]
        [Required(ErrorMessage = "You must provide a first name")]
        public string FirstNames { get; set; }

        [DisplayName("Surname")]
        [Required(ErrorMessage = "You must provide a surname")]
        public string Surname { get; set; }

        public string OperatorNumber { get; set; }
        public string FlyerNumber { get; set; }

        public DateTime DoB { get; set; }
        public Address Address { get; set; }

        public string DoBFormatted
        {
            get
            {
                return DoB.ToString("dd-MMM-yyyy");
            }
        }

        /// <summary>
        /// Return a one-line summary of  the address
        /// </summary>
        /// <returns></returns>
        public string AddressSummary
        {
            get
            {
                return Address != null ? $"{Address.Number} {Address.Street}, {Address.Postcode}, {Address.Country}" : "";
            }
        }
    }
}
