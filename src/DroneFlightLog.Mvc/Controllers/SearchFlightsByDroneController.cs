using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Configuration;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class SearchFlightsByDroneController : Controller
    {
        private readonly DroneClient _drones;
        private readonly FlightSearchClient _flights;
        private readonly IOptions<AppSettings> _settings;

        public SearchFlightsByDroneController(DroneClient drones, FlightSearchClient flights, IOptions<AppSettings> settings)
        {
            _drones = drones;
            _flights = flights;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search flights by drone page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            FlightSearchByDroneViewModel model = new FlightSearchByDroneViewModel
            {
                PageNumber = 1
            };
            List<Drone> drones = await _drones.GetDronesAsync();
            model.SetDrones(drones);
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FlightSearchByDroneViewModel model)
        {
            if (ModelState.IsValid)
            {
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        model.PageNumber -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        model.PageNumber += 1;
                        break;
                    default:
                        break;
                }

                List<Flight> flights = await _flights.GetFlightsByDroneAsync(model.DroneId, model.PageNumber, _settings.Value.FlightSearchPageSize);
                model.SetFlights(flights, _settings.Value.FlightSearchPageSize);
            }

            List<Drone> drones = await _drones.GetDronesAsync();
            model.SetDrones(drones);

            return View(model);
        }

    }
}
