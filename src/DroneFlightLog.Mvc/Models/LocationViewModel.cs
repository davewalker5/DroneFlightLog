using DroneFlightLog.Mvc.Entities;

namespace DroneFlightLog.Mvc.Models
{
    public class LocationViewModel : Location
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
