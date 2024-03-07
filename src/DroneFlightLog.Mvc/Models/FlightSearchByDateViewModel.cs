using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightSearchByDateViewModel : FlightSearchBaseViewModel
    {
        [DisplayName("From")]
        [Required(ErrorMessage = "You must enter a 'from' date")]
        public string From { get; set; }

        [DisplayName("To")]
        [Required(ErrorMessage = "You must enter a 'to' date")]
        public string To { get; set; }
    }
}
