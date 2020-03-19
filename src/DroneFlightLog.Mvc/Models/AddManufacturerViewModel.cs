using DroneFlightLog.Mvc.Entities;

namespace DroneFlightLog.Mvc.Models
{
    public class AddManufacturerViewModel : Manufacturer
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            Name = "";
            Message = "";
        }
    }
}
