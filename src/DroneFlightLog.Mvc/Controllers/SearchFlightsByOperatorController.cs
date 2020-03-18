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
        private readonly DroneFlightLogClient _client;
        private readonly IOptions<AppSettings> _settings;

        public SearchFlightsByOperatorController(OperatorClient operators, DroneFlightLogClient client, IOptions<AppSettings> settings)
        {
            _operators = operators;
            _client = client;
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

                List<Flight> flights = await _client.GetFlightsByOperatorAsync(model.OperatorId, model.PageNumber, _settings.Value.FlightSearchPageSize);
                model.SetFlights(flights, _settings.Value.FlightSearchPageSize);
            }

            List<Operator> operators = await _operators.GetOperatorsAsync();
            model.SetOperators(operators);

            return View(model);
        }

    }
}
