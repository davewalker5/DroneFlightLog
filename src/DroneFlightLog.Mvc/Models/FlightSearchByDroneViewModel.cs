using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightSearchByDroneViewModel : FlightSearchBaseViewModel
    {
        [DisplayName("Drone")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a drone")]
        [Required]
        public int DroneId { get; set; }

        public List<SelectListItem> Drones { get; private set; }

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
    }
}
