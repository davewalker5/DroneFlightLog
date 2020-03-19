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
    public class DronesController : Controller
    {
        private readonly ModelClient _models;
        private readonly DroneClient _drones;
        private readonly IMapper _mapper;

        public DronesController(ModelClient models, DroneClient drones, IMapper mapper)
        {
            _models = models;
            _drones = drones;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the drone list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Drone> drones = await _drones.GetDronesAsync();
            return View(drones);
        }

        /// <summary>
        /// Serve the page to add a new drone
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Model> models = await _models.GetModelsAsync();
            AddDroneViewModel model = new AddDroneViewModel();
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
        public async Task<IActionResult> Add(AddDroneViewModel droneModel)
        {
            if (ModelState.IsValid)
            {
                Drone drone = await _drones.AddDroneAsync(droneModel.Name, droneModel.SerialNumber, droneModel.ModelId);
                ModelState.Clear();
                droneModel.Clear();
                droneModel.Message = $"Drone '{drone.Name}' added successfully";
            }

            List<Model> models = await _models.GetModelsAsync();
            droneModel.SetModels(models);

            return View(droneModel);
        }

        /// <summary>
        /// Serve the drone editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Drone drone = await _drones.GetDroneAsync(id);
            List<Model> models = await _models.GetModelsAsync();
            EditDroneViewModel model = _mapper.Map<EditDroneViewModel>(drone);
            model.SetModels(models);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated drones
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDroneViewModel droneModel)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _drones.UpdateDroneAsync(droneModel.Id, droneModel.Name, droneModel.SerialNumber, droneModel.ModelId);
                result = RedirectToAction("Index");
            }
            else
            {
                List<Model> models = await _models.GetModelsAsync();
                droneModel.SetModels(models);
                result = View(droneModel);
            }

            return result;
        }
    }
}
