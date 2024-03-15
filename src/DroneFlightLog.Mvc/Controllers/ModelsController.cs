using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public ModelsController(ManufacturerClient manufacturers, ModelClient models, IMapper mapper)
        {
            _manufacturers = manufacturers;
            _models = models;
            _mapper = mapper;
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
            var model = new AddModelViewModel();
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
        public async Task<IActionResult> Add(AddModelViewModel model)
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

        /// <summary>
        /// Serve the location editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Model droneModel = await _models.GetModelAsync(id);
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
            EditModelViewModel model = _mapper.Map<EditModelViewModel>(droneModel);
            model.SetManufacturers(manufacturers);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated locations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditModelViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _models.UpdateModelAsync(model.Id, model.ManufacturerId, model.Name);
                result = RedirectToAction("Index");
            }
            else
            {
                List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
                model.SetManufacturers(manufacturers);
                result = View(model);
            }

            return result;
        }
    }
}
