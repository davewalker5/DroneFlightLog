using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class FlightsController : Controller
    {
        private readonly DroneClient _drones;
        private readonly LocationClient _locations;
        private readonly OperatorClient _operators;
        private readonly FlightClient _flights;
        private readonly IMapper _mapper;

        public FlightsController(DroneClient drones, LocationClient locations, OperatorClient operators, FlightClient flights, IMapper mapper)
        {
            _drones = drones;
            _locations = locations;
            _operators = operators;
            _flights = flights;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the page to add a new flight
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Drone> drones = await _drones.GetDronesAsync();
            List<Location> locations = await _locations.GetLocationsAsync();
            List<Operator> operators = await _operators.GetOperatorsAsync();

            AddFlightViewModel model = new();
            model.SetDrones(drones);
            model.SetLocations(locations);
            model.SetOperators(operators);

            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new flights
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddFlightViewModel model)
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

        /// <summary>
        /// Serve the page to edit an existing flight
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            List<Drone> drones = await _drones.GetDronesAsync();
            List<Location> locations = await _locations.GetLocationsAsync();
            List<Operator> operators = await _operators.GetOperatorsAsync();

            Flight flight = await _flights.GetFlightAsync(id);
            EditFlightViewModel model = _mapper.Map<EditFlightViewModel>(flight);

            model.SetDrones(drones);
            model.SetLocations(locations);
            model.SetOperators(operators);

            model.StartDate = model.Start.ToString("MM/dd/yyyy");
            model.StartTime = model.Start.ToString("HH:mm");
            model.EndDate = model.End.ToString("MM/dd/yyyy");
            model.EndTime = model.Start.ToString("HH:mm");

            return View(model);
        }

        /// <summary>
        /// Handle POST events to update existing flights
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFlightViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                DateTime start = CombineDateAndTime(model.StartDate, model.StartTime);
                DateTime end = CombineDateAndTime(model.EndDate, model.EndTime);

                Flight flight = await _flights.UpdateFlightAsync(
                                                model.Id,
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

        /// <summary>
        /// Combine a date string and a time string into a single date time
        /// </summary>
        /// <param name="dateString"></param>
        /// <param name="timeString"></param>
        /// <returns></returns>
        private static DateTime CombineDateAndTime(string dateString, string timeString)
        {
            var date = DateTime.Parse(dateString);
            var time = DateTime.ParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture);
            var final = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
            return final;
        }
    }
}
