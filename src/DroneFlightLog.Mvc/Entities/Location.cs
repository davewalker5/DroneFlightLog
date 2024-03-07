using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class Location
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
    }
}
