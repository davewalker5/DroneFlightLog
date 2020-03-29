using System.Collections.Generic;
using DroneFlightLog.Mvc.Entities;

namespace DroneFlightLog.Mvc.Models
{
    public abstract class FlightSearchBaseViewModel
    {
        public List<Flight> Flights { get; private set; }
        public int PageNumber { get; set; }
        public bool PreviousEnabled { get; private set; }
        public bool NextEnabled { get; private set; }
        public string Action { get; set; }

        /// <summary>
        /// Set the collection of flights that are exposed to the view
        /// </summary>
        /// <param name="flights"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetFlights(List<Flight> flights, int pageNumber, int pageSize)
        {
            Flights = flights;
            PageNumber = pageNumber;
            SetPreviousNextEnabled(pageNumber, pageSize);
        }

        /// <summary>
        /// Set the "previous/next" button enabled flags according to the
        /// following logic, where SZ is the page size:
        ///
        /// Flight  Page    Previous    Next
        /// Count   Number  Enabled     Enabled
        ///
        /// 0       -       No          No
        /// = SZ    1       No          Yes   
        /// < SZ    1       No          No
        /// = SZ    > 1     Yes         Yes
        /// < SZ    > 1     Yes         No
        /// 
        /// </summary>
        /// <param name="pageNumber"/>
        /// <param name="pageSize"/>
        /// <returns></returns>
        private void SetPreviousNextEnabled(int pageNumber, int pageSize)
        {
            int count = Flights?.Count ?? 0;
            PreviousEnabled = (pageNumber > 1);
            NextEnabled = (count == pageSize);
        }
    }
}
