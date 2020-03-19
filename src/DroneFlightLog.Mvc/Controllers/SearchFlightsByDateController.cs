using System;
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
    public class SearchFlightsByDateController : Controller
    {
        private readonly FlightSearchClient _client;
        private readonly IOptions<AppSettings> _settings;

        public SearchFlightsByDateController(FlightSearchClient client, IOptions<AppSettings> settings)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search flights by date page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            FlightSearchByDateViewModel model = new FlightSearchByDateViewModel
            {
                PageNumber = 1
            };
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FlightSearchByDateViewModel model)
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

                DateTime start = DateTime.Parse(model.From);
                DateTime end = DateTime.Parse(model.To);
                List<Flight> flights = await _client.GetFlightsByDateAsync(start, end, model.PageNumber, _settings.Value.FlightSearchPageSize);
                model.SetFlights(flights, _settings.Value.FlightSearchPageSize);
            }

            return View(model);
        }

    }
}
