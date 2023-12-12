using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightSearchByLocationViewModel : FlightSearchBaseViewModel
    {
        [DisplayName("Location")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a flight location")]
        [Required]
        public int LocationId { get; set; }

        public List<SelectListItem> Locations { get; private set; }

        /// <summary>
        /// Set the options for the lcoations drop-down list
        /// </summary>
        /// <param name="locations"></param>
        public void SetLocations(List<Location> locations)
        {
            // Add the default selection, which is empty
            Locations = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the locations retrieved from the service
            Locations.AddRange(locations.Select(l =>
                                new SelectListItem
                                {
                                    Value = l.Id.ToString(),
                                    Text = l.Name
                                }));
        }
    }
}
