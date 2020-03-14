using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class DronesController : Controller
    {
        private readonly DroneFlightLogClient _client;

        public DronesController(DroneFlightLogClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the drone list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Drone> drones = await _client.GetDronesAsync();
            return View(drones);
        }

        /// <summary>
        /// Serve the page to add a new drone
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Model> models = await _client.GetModelsAsync();
            DroneViewModel model = new DroneViewModel();
            model.SetModels(models);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new drones
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DroneViewModel droneModel)
        {
            if (ModelState.IsValid)
            {
                Drone drone = await _client.AddDroneAsync(droneModel.Name, droneModel.SerialNumber, droneModel.ModelId);
                ModelState.Clear();
                droneModel.Clear();
                droneModel.Message = $"Model '{drone.Name}' added successfully";
            }

            List<Model> models = await _client.GetModelsAsync();
            droneModel.SetModels(models);

            return View(droneModel);
        }
    }
}
