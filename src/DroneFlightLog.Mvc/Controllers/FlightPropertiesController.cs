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
    public class FlightPropertiesController : Controller
    {
        private readonly FlightPropertyClient _client;
        private readonly IMapper _mapper;

        public FlightPropertiesController(FlightPropertyClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the flight properties list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<FlightProperty> properties = await _client.GetFlightPropertiesAsync();
            return View(properties);
        }

        /// <summary>
        /// Serve the page to add a new model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddFlightPropertyViewModel());
        }

        /// <summary>
        /// Handle POST events to save new models
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddFlightPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                FlightProperty property = await _client.AddFlightPropertyAsync(model.Name, model.DataTypeValue, model.IsSingleInstance);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Flight property '{property.Name}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the flight property editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            FlightProperty property = await _client.GetFlightPropertyAsync(id);
            EditFlightPropertyViewModel model = _mapper.Map<EditFlightPropertyViewModel>(property);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated flight properties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFlightPropertyViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateFlightPropertyAsync(model.Id, model.Name);
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
