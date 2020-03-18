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
    public class AllFlightsController : Controller
    {
        private readonly FlightSearchClient _client;
        private readonly IOptions<AppSettings> _settings;

        public AllFlightsController(FlightSearchClient client, IOptions<AppSettings> settings)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the (empty) flight viewing page to list all flights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AllFlightsViewModel model = new AllFlightsViewModel { PageNumber = 1 };
            List<Flight> flights = await _client.GetFlightsAsync(1, _settings.Value.FlightSearchPageSize);
            model.SetFlights(flights, _settings.Value.FlightSearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search. In this case, the outcome
        /// is simply page navigation rather than filtering
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AllFlightsViewModel model)
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

            List<Flight> flights = await _client.GetFlightsAsync(model.PageNumber, _settings.Value.FlightSearchPageSize);
            model.SetFlights(flights, _settings.Value.FlightSearchPageSize);
            return View(model);
        }
    }
}
