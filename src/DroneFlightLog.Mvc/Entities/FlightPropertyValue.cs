using System;

namespace DroneFlightLog.Mvc.Entities
{
    public class FlightPropertyValue
    {
        public const string ControlNamePrefix = "FPV_";

        #region Entity Properties From The Service
        public int Id { get; set; }
        public string StringValue { get; set; }
        public DateTime? DateValue { get; set; }
        public decimal? NumberValue { get; set; }
        public FlightProperty Property { get; set; }
        #endregion

        #region Flight Details Presentation Properties
        public string ControlName { get { return $"{ControlNamePrefix}{Property.Id.ToString()}"; } }

        public string DateValueFormatted
        {
            get
            {
                return (DateValue != null) ? (DateValue ?? DateTime.Now).ToString("dd-MMM-yyyy") : "";
            }
        }

        public bool IsNewPropertyValue
        {
            get
            {
                return (Id == 0) &&
                       (!string.IsNullOrEmpty(StringValue) ||
                        (DateValue != null) ||
                        (NumberValue != null));
            }
        }
        #endregion
    }
}
