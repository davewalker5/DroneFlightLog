using DroneFlightLog.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DroneFlightLog.Mvc.Models
{
    public class MaintenanceRecordViewModelBase : MaintenanceRecord
    {
        public List<SelectListItem> Drones { get; private set; }
        public List<SelectListItem> Maintainers { get; private set; }
        public List<SelectListItem> MaintenanceTypes { get; private set; }

        public MaintenanceRecordViewModelBase()
        {
            MaintenanceTypes =
            [
                new SelectListItem { Value = "", Text = "" }
            ];

            foreach (var type in Enum.GetValues(typeof(MaintenanceRecordType)))
            {
                MaintenanceTypes.Add(new SelectListItem { Value = type.ToString(), Text = type.ToString() });
            }   
        }

        /// <summary>
        /// Reset the model
        /// </summary>
        public void Clear()
        {
            DroneId = 0;
            MaintainerId = 0;
            RecordType = MaintenanceRecordType.Maintenance;
            Description = "";
            Notes = "";
            DateWorkCompleted = "";
        }

        /// <summary>
        /// Construct the options for the drones drop-down
        /// </summary>
        /// <param name="drones"></param>
        public void SetDrones(List<Drone> drones)
        {
            // Add the default selection, which is empty
            Drones =
            [
                new SelectListItem{ Value = "", Text = "" }
            ];

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
        /// Construct the options for the maintainers drop-down
        /// </summary>
        /// <param name="drones"></param>
        public void SetMaintainers(List<Maintainer> maintainers)
        {
            // Add the default selection, which is empty
            Maintainers =
            [
                new SelectListItem{ Value = "", Text = "" }
            ];

            if (maintainers?.Count > 0)
            {
                Maintainers.AddRange(maintainers.Select(m =>
                                    new SelectListItem
                                    {
                                        Value = m.Id.ToString(),
                                        Text = m.MaintainerFullName
                                    }));
            }
        }
    }
}
