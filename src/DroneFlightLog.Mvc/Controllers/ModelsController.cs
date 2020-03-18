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
        private readonly ManufacturerClient _manufacturers;
        private readonly ModelClient _models;

        public ModelsController(ManufacturerClient manufacturers, ModelClient models)
        {
            _manufacturers = manufacturers;
            _models = models;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Model> models = await _models.GetModelsAsync();
            return View(models);
        }

        /// <summary>
        /// Serve the page to add a new model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
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
                Model droneModel = await _models.AddModelAsync(model.Name, model.ManufacturerId);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Model '{droneModel.Name}' added successfully";
            }

            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
            model.SetManufacturers(manufacturers);

            return View(model);
        }
    }
}
