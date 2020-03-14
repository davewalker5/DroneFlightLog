using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public class FlightViewModel : Flight
    {
        public List<SelectListItem> Drones { get; private set; }
        public List<SelectListItem> Locations { get; private set; }
        public List<SelectListItem> Operators { get; private set; }

        /// <summary>
        /// Reset the model
        /// </summary>
        public void Clear()
        {
            LocationId = 0;
            OperatorId = 0;
            DroneId = 0;
            StartDate = "";
            StartTime = "";
            EndDate = "";
            EndTime = "";
        }

        /// <summary>
        /// Construct the options for the drones drop-down
        /// </summary>
        /// <param name="drones"></param>
        public void SetDrones(List<Drone> drones)
        {
            // Add the default selection, which is empty
            Drones = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the models retrieved from the service
            Drones.AddRange(drones.Select(d =>
                                new SelectListItem
                                {
                                    Value = d.Id.ToString(),
                                    Text = $"{d.Name} - {d.Model.Name}"
                                }));
        }

        /// <summary>
        /// Construct the options for the locations drop-down
        /// </summary>
        /// <param name="locations"></param>
        public void SetLocations(List<Location> locations)
        {
            // Add the default selection, which is empty
            Locations = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the models retrieved from the service
            Locations.AddRange(locations.Select(l =>
                                new SelectListItem
                                {
                                    Value = l.Id.ToString(),
                                    Text = l.Name
                                }));
        }

        /// <summary>
        /// Construct the options for the operators drop-down
        /// </summary>
        /// <param name="operators"></param>
        public void SetOperators(List<Operator> operators)
        {
            // Add the default selection, which is empty
            Operators = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the models retrieved from the service
            Operators.AddRange(operators.Select(o =>
                                new SelectListItem
                                {
                                    Value = o.Id.ToString(),
                                    Text = $"{o.FirstNames} {o.Surname} - {o.OperatorNumber}"
                                }));
        }
    }
}
