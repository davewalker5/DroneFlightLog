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
    public class ModelsController : Controller
    {
        private readonly DroneFlightLogClient _client;

        public ModelsController(DroneFlightLogClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Model> models = await _client.GetModelsAsync();
            return View(models);
        }

        /// <summary>
        /// Serve the page to add a new model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Manufacturer> manufacturers = await _client.GetManufacturersAsync();
            ModelViewModel model = new ModelViewModel();
            model.SetManufacturers(manufacturers);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new models
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ModelViewModel model)
        {
            if (ModelState.IsValid)
            {
                Model droneModel = await _client.AddModelAsync(model.Name, model.ManufacturerId);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Model '{droneModel.Name}' added successfully";
            }

            List<Manufacturer> manufacturers = await _client.GetManufacturersAsync();
            model.SetManufacturers(manufacturers);

            return View(model);
        }
    }
}
