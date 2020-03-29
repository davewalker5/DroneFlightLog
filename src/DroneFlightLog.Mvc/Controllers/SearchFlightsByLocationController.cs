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
    public class SearchFlightsByLocationController : Controller
    {
        private readonly LocationClient _locations;
        private readonly FlightSearchClient _flights;
        private readonly IOptions<AppSettings> _settings;

        public SearchFlightsByLocationController(LocationClient locations, FlightSearchClient flights, IOptions<AppSettings> settings)
        {
            _locations = locations;
            _flights = flights;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search flights by location page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            FlightSearchByLocationViewModel model = new FlightSearchByLocationViewModel
            {
                PageNumber = 1
            };
            List<Location> locations = await _locations.GetLocationsAsync();
            model.SetLocations(locations);
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FlightSearchByLocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                List<Flight> flights = await _flights.GetFlightsByLocationAsync(model.LocationId, model.PageNumber, _settings.Value.FlightSearchPageSize);
                model.SetFlights(flights, _settings.Value.FlightSearchPageSize);
                model.PageNumber = page;
            }

            List<Location> locations = await _locations.GetLocationsAsync();
            model.SetLocations(locations);

            return View(model);
        }

    }
}
