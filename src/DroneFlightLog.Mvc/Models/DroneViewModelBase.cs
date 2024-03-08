using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public abstract class DroneViewModelBase : Drone
    {
        public List<SelectListItem> Models { get; set; }

        /// <summary>
        /// Construct the options for the models drop-down
        /// </summary>
        /// <param name="models"></param>
        public void SetModels(List<Model> models)
        {
            // Add the default selection, which is empty
            Models = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the models retrieved from the service
            if (models?.Count > 0)
            {
                Models.AddRange(models.Select(m =>
                                    new SelectListItem
                                    {
                                        Value = m.Id.ToString(),
                                        Text = $"{m.Manufacturer.Name} - {m.Name}"
                                    }));
            }
        }
    }
}
