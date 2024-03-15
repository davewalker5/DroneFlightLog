using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DroneFlightLog.Mvc.Controllers
{
    public class MaintenanceRecordsSearchController : Controller
    {
        private readonly DroneClient _drones;
        private readonly MaintenanceRecordClient _maintenanceRecords;
        private readonly IOptions<AppSettings> _settings;

        public MaintenanceRecordsSearchController(DroneClient drones, MaintenanceRecordClient maintenanceRecords, IOptions<AppSettings> settings)
        {
            _drones = drones;
            _maintenanceRecords = maintenanceRecords;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty page to for search maintenance records
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int? droneId)
        {
            MaintenanceRecordsSearchViewModel model = new()
            {
                PageNumber = 1
            };

            var drones = await _drones.GetDronesAsync();
            model.SetDrones(drones);

            // If we have a drone ID, apply it to the model and retrieve maintenance records for that drone
            if (droneId > 0)
            {
                model.DroneId = droneId.Value;
                var maintenanceRecords = await _maintenanceRecords.GetMaintenanceRecordsForDoneAsync(model.DroneId, null, null, model.PageNumber, _settings.Value.MaintenanceRecordSearchPageSize);
                model.SetMaintenanceRecords(maintenanceRecords, model.PageNumber, _settings.Value.MaintenanceRecordSearchPageSize);
            }

            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MaintenanceRecordsSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    case ControllerActions.ActionSearch:
                        page = 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                DateTime? start = !string.IsNullOrEmpty(model.From) ? DateTime.Parse(model.From) : null;
                DateTime? end = !string.IsNullOrEmpty(model.To) ? DateTime.Parse(model.To) : null;
                var maintenanceRecords = await _maintenanceRecords.GetMaintenanceRecordsForDoneAsync(model.DroneId, start, end, page, _settings.Value.MaintenanceRecordSearchPageSize);
                model.SetMaintenanceRecords(maintenanceRecords, page, _settings.Value.MaintenanceRecordSearchPageSize);
            }

            var drones = await _drones.GetDronesAsync();
            model.SetDrones(drones);

            return View(model);
        }
    }
}
