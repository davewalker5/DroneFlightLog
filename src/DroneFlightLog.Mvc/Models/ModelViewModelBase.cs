using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public abstract class ModelViewModelBase : Model
    {
        public List<SelectListItem> Manufacturers { get; set; }

        /// <summary>
        /// Construct the options for the manufacturers drop-down
        /// </summary>
        /// <param name="manufacturers"></param>
        public void SetManufacturers(List<Manufacturer> manufacturers)
        {
            // Add the default selection, which is empty
            Manufacturers = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the manufacturers retrieved from the service
            if (manufacturers?.Count > 0)
            {
                Manufacturers.AddRange(manufacturers.Select(m =>
                                    new SelectListItem
                                    {
                                        Value = m.Id.ToString(),
                                        Text = $"{m.Name}"
                                    }));
            }
        }
    }
}
