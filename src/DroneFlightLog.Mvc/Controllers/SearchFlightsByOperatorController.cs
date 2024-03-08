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
    public class SearchFlightsByOperatorController : Controller
    {
        private readonly OperatorClient _operators;
        private readonly FlightSearchClient _flights;
        private readonly IOptions<AppSettings> _settings;

        public SearchFlightsByOperatorController(OperatorClient operators, FlightSearchClient flights, IOptions<AppSettings> settings)
        {
            _operators = operators;
            _flights = flights;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search flights by operator page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            FlightSearchByOperatorViewModel model = new FlightSearchByOperatorViewModel
            {
                PageNumber = 1
            };
            List<Operator> operators = await _operators.GetOperatorsAsync();
            model.SetOperators(operators);
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FlightSearchByOperatorViewModel model)
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
                    case ControllerActions.ActionSearch:
                        page = 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                List<Flight> flights = await _flights.GetFlightsByOperatorAsync(model.OperatorId, page, _settings.Value.FlightSearchPageSize);
                model.SetFlights(flights, page, _settings.Value.FlightSearchPageSize);
            }

            List<Operator> operators = await _operators.GetOperatorsAsync();
            model.SetOperators(operators);

            return View(model);
        }

    }
}
