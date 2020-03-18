using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DroneFlightLog.Mvc.Models
{
    public class AddFlightPropertyViewModel : FlightPropertyViewModelBase
    {
        public string Message { get; set; }

        [DisplayName("Data Type")]
        [Range(0, 2, ErrorMessage = "You must select a data type")]
        public int DataTypeValue { get; set; }

        public AddFlightPropertyViewModel() : base()
        {
        }

        /// <summary>
        /// Reset the view model
        /// </summary>
        public void Clear()
        {
            Id = 0;
            DataTypeValue = 0;
            Name = "";
            IsSingleInstance = false;
            Message = "";
        }
    }
}
