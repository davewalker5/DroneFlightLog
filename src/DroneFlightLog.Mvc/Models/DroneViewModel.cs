using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DroneFlightLog.Mvc.Models
{
    public class DroneViewModel : Drone
    {
        public List<SelectListItem> Models { get; private set; }
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
            Models.AddRange(models.Select(m =>
                                new SelectListItem
                                {
                                    Value = m.Id.ToString(),
                                    Text = $"{m.Manufacturer.Name} - {m.Name}"
                                }));
        }
    }
}
