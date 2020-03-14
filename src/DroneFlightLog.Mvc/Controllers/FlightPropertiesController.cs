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
    public class FlightPropertiesController : Controller
    {
        private readonly DroneFlightLogClient _client;

        public FlightPropertiesController(DroneFlightLogClient client)
        {
            _client = client;
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
            return View(new FlightPropertyViewModel());
        }

        /// <summary>
        /// Handle POST events to save new models
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(FlightPropertyViewModel model)
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
    }
}
