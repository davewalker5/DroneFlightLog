using AutoMapper;
using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneFlightLog.Mvc.Controllers
{
    public class MaintenanceRecordsController : Controller
    {
        private readonly DroneClient _drones;
        private readonly MaintainersClient _maintainers;
        private readonly MaintenanceRecordClient _maintenanceRecords;
        private readonly IMapper _mapper;

        public MaintenanceRecordsController(DroneClient drones, MaintainersClient maintainers, MaintenanceRecordClient maintenanceRecords, IMapper mapper)
        {
            _drones = drones;
            _maintainers = maintainers;
            _maintenanceRecords = maintenanceRecords;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the page to add a new maintenance record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Drone> drones = await _drones.GetDronesAsync();
            List<Maintainer> maintainers = await _maintainers.GetMaintainersAsync();

            AddMaintenanceRecordViewModel model = new();
            model.SetDrones(drones);
            model.SetMaintainers(maintainers);

            return View(model);
        }

        /// <summary>
        /// Handle POST events to add new maintenance records
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMaintenanceRecordViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                DateTime dateCompleted = DateTime.Parse(model.DateWorkCompleted);

                MaintenanceRecord maintenanceRecord = await _maintenanceRecords.AddMaintenanceRecordAsync(
                                                                model.MaintainerId,
                                                                model.DroneId,
                                                                dateCompleted,
                                                                model.RecordType,
                                                                model.Description,
                                                                model.Notes);

                // Redirect to the mainteance records search page
                result = RedirectToAction("Index", "MaintenanceRecordsSearch", new { droneId = maintenanceRecord.DroneId });
            }
            else
            {
                // If the model state isn't valid, load the lists of drones and maintainers and
                // redisplay the editing page
                List<Drone> drones = await _drones.GetDronesAsync();
                List<Maintainer> maintainers = await _maintainers.GetMaintainersAsync();

                model.SetDrones(drones);
                model.SetMaintainers(maintainers);

                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Serve the page to edit an existing maintenance record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            List<Drone> drones = await _drones.GetDronesAsync();
            List<Maintainer> maintainers = await _maintainers.GetMaintainersAsync();

            MaintenanceRecord maintenanceRecord = await _maintenanceRecords.GetMaintenanceRecordAsync(id);
            EditMaintenanceRecordViewModel model = _mapper.Map<EditMaintenanceRecordViewModel>(maintenanceRecord);

            model.SetDrones(drones);
            model.SetMaintainers(maintainers);

            model.DateWorkCompleted = model.DateCompleted.ToString("MM/dd/yyyy");

            return View(model);
        }

        /// <summary>
        /// Handle POST events to update existing maintenance records
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMaintenanceRecordViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                DateTime dateCompleted = DateTime.Parse(model.DateWorkCompleted);

                MaintenanceRecord maintenanceRecord = await _maintenanceRecords.UpdateMaintenanceRecordAsync(
                                                                model.Id,
                                                                model.MaintainerId,
                                                                model.DroneId,
                                                                dateCompleted,
                                                                model.RecordType,
                                                                model.Description,
                                                                model.Notes);

                // Redirect to the mainteance records search page
                result = RedirectToAction("Index", "MaintenanceRecordsSearch", new { droneId = maintenanceRecord.DroneId });
            }
            else
            {
                // If the model state isn't valid, load the lists of drones and maintainers and
                // redisplay the editing page
                List<Drone> drones = await _drones.GetDronesAsync();
                List<Maintainer> maintainers = await _maintainers.GetMaintainersAsync();

                model.SetDrones(drones);
                model.SetMaintainers(maintainers);

                result = View(model);
            }

            return result;
        }
    }
}
