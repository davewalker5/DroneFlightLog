using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DroneFlightLog.Mvc.Models
{
    public class MaintenanceRecordsSearchViewModel
    {
        [DisplayName("Drone")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a drone")]
        [Required]
        public int DroneId { get; set; }

        [DisplayName("From")]
        public string From { get; set; }

        [DisplayName("To")]
        public string To { get; set; }
        public int PageNumber { get; set; }
        public bool PreviousEnabled { get; private set; }
        public bool NextEnabled { get; private set; }
        public string Action { get; set; }

        public List<SelectListItem> Drones { get; private set; }
        public List<MaintenanceRecord> MaintenanceRecords { get; private set; }

        /// <summary>
        /// Set the options for the drones drop-down list
        /// </summary>
        /// <param name="drones"></param>
        public void SetDrones(List<Drone> drones)
        {
            // Add the default selection, which is empty
            Drones =
            [
                new() { Value = "", Text = "" }
            ];

            // Add the drones retrieved from the service
            if (drones?.Count > 0)
            {
                Drones.AddRange(drones.Select(d =>
                                    new SelectListItem
                                    {
                                        Value = d.Id.ToString(),
                                        Text = $"{d.Name} - {d.Model.Name}"
                                    }));
            }
        }

        /// <summary>
        /// Set the collection of maintenance records that are exposed to the view
        /// </summary>
        /// <param name="records"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetMaintenanceRecords(List<MaintenanceRecord> records, int pageNumber, int pageSize)
        {
            MaintenanceRecords = records;
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
            int count = MaintenanceRecords?.Count ?? 0;
            PreviousEnabled = (pageNumber > 1);
            NextEnabled = (count == pageSize);
        }
    }
}
