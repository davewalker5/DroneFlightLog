namespace DroneFlightLog.Mvc.Models
{
    public class AddDroneViewModel : DroneViewModelBase
    {
        public string Message { get; set; }

        /// <summary>
        /// Reset the view model
        /// </summary>
        public void Clear()
        {
            Id = 0;
            ModelId = 0;
            Name = "";
            SerialNumber = "";
            Models = null;
            Message = "";
        }
    }
}
