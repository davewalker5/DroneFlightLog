using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Entities
{
    public class FlightProperty
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        [DisplayName("Data Type")]
        public FlightPropertyDataType DataType { get; set; }

        [DisplayName("Single Instance")]
        public bool IsSingleInstance { get; set; }

        public string IsSingleInstanceYesNo { get {  return (IsSingleInstance) ? "Yes" : "No"; } }
    }
}
