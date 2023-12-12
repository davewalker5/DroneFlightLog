using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class Drone
    {
        public int Id { get; set; }

        [DisplayName("Model")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a model")]
        public int ModelId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        [DisplayName("Serial Number")]
        [Required(ErrorMessage = "You must provide a serial number")]
        public string SerialNumber { get; set; }

        public Model Model { get; set; }
    }
}
