using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DroneClient _drones;
        private readonly LocationClient _locations;
        private readonly OperatorClient _operators;
        private readonly FlightClient _flights;

        public HomeController(DroneClient drones, LocationClient locations, OperatorClient operators, FlightClient flights)
        {
            _drones = drones;
            _locations = locations;
            _operators = operators;
            _flights = flights;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Drone> drones = await _drones.GetDronesAsync();
            List<Location> locations = await _locations.GetLocationsAsync();
            List<Operator> operators = await _operators.GetOperatorsAsync();

            FlightViewModel model = new FlightViewModel();
            model.SetDrones(drones);
            model.SetLocations(locations);
            model.SetOperators(operators);

            return View(model);
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FlightViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                DateTime start = CombineDateAndTime(model.StartDate, model.StartTime);
                DateTime end = CombineDateAndTime(model.EndDate, model.EndTime);

                Flight flight = await _flights.AddFlightAsync(
                                                model.DroneId,
                                                model.LocationId,
                                                model.OperatorId,
                                                start,
                                                end);

                // Redirect to the flight details/properties page
                result = RedirectToAction("Index", "FlightDetails", new { id = flight.Id });
            }
            else
            {
                // If the model state isn't valid, load the lists of drones,
                // locations and operators and redisplay the flight logging page
                List<Drone> drones = await _drones.GetDronesAsync();
                List<Location> locations = await _locations.GetLocationsAsync();
                List<Operator> operators = await _operators.GetOperatorsAsync();

                model.SetDrones(drones);
                model.SetLocations(locations);
                model.SetOperators(operators);

                result = View(model);
            }

            return result;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Combine a date string and a time string into a single date time
        /// </summary>
        /// <param name="dateString"></param>
        /// <param name="timeString"></param>
        /// <returns></returns>
        private DateTime CombineDateAndTime(string dateString, string timeString)
        {
            DateTime date = DateTime.Parse(dateString);
            DateTime time = DateTime.ParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture);
            DateTime final = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
            return final;
        }
    }
}
