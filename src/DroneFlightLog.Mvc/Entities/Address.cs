using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class Address
    {
        public int Id { get; set; }

        [DisplayName("House Number")]
        [Required(ErrorMessage = "You must provide a house number")]
        public string Number { get; set; }

        [DisplayName("Street")]
        [Required(ErrorMessage = "You must provide a street")]
        public string Street { get; set; }

        [DisplayName("Town")]
        [Required(ErrorMessage = "You must provide a town")]
        public string Town { get; set; }

        [DisplayName("County / State")]
        [Required(ErrorMessage = "You must provide a county/state")]
        public string County { get; set; }

        [DisplayName("Postcode / ZIP Code")]
        [Required(ErrorMessage = "You must provide a postcode/ZIP code")]
        public string Postcode { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "You must provide a country")]
        public string Country { get; set; }
    }
}
