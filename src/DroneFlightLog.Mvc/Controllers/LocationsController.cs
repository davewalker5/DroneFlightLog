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
    public class LocationsController : Controller
    {
        private readonly LocationClient _client;
        private readonly IMapper _mapper;

        public LocationsController(LocationClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the locations list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Location> locations = await _client.GetLocationsAsync();
            return View(locations);
        }

        /// <summary>
        /// Serve the page to add a new location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddLocationViewModel());
        }

        /// <summary>
        /// Handle POST events to save new locations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddLocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                Location location = await _client.AddLocationAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Location '{location.Name}' added successfully";
            }

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
            Location location = await _client.GetLocation(id);
            EditLocationViewModel model = _mapper.Map<EditLocationViewModel>(location);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated locations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditLocationViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateLocationAsync(model.Id, model.Name);
                result = RedirectToAction("Index");
            }
            else
            {
                result = View(model);
            }

            return result;
        }
    }
}
