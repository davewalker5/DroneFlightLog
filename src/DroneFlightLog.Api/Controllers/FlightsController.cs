using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DroneFlightLog.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class FlightsController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public FlightsController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{page}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsAsync(int page, int pageSize)
        {
            List<Flight> flights = await _factory.Flights.FindFlightsAsync(null, null, null, null, null, page, pageSize).ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("operator/{operatorId}/{page}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> FindFlightsByOperatorAsync(int operatorId, int page, int pageSize)
        {
            List<Flight> flights = await _factory.Flights.FindFlightsAsync(operatorId, null, null, null, null, page, pageSize).ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("drone/{droneId}/{page}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> FindFlightsByDroneAsync(int droneId, int page, int pageSize)
        {
            List<Flight> flights = await _factory.Flights.FindFlightsAsync(null, droneId, null, null, null, page, pageSize).ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("location/{locationId}/{page}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> FindFlightsByLocationAsync(int locationId, int page, int pageSize)
        {
            List<Flight> flights = await _factory.Flights.FindFlightsAsync(null, null, locationId, null, null, page, pageSize).ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("date/{start}/{end}/{page}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> FindFlightsByDateAsync(string start, string end, int page, int pageSize)
        {
            DateTime flightStart = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime flightEnd = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            List<Flight> flights = await _factory.Flights.FindFlightsAsync(null, null, null, flightStart, flightEnd, page, pageSize).ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Flight>> UpdateFlightAsync([FromBody] Flight template)
        {
            Flight flight;

            try
            {
                flight = await _factory.Flights.UpdateFlightAsync(template.Id,
                                                                  template.OperatorId,
                                                                  template.DroneId,
                                                                  template.LocationId,
                                                                  template.Start,
                                                                  template.End);
                await _factory.Context.SaveChangesAsync();
            }
            catch (OperatorNotFoundException)
            {
                return BadRequest();
            }
            catch (DroneNotFoundException)
            {
                return BadRequest();
            }
            catch (LocationNotFoundException)
            {
                return BadRequest();
            }
            catch (FlightNotFoundException)
            {
                return NotFound();
            }

            return flight;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Flight>> CreateFlightAsync([FromBody] Flight template)
        {
            Flight flight;

            try
            {
                flight = await _factory.Flights.AddFlightAsync(template.OperatorId,
                                                                template.DroneId,
                                                                template.LocationId,
                                                                template.Start,
                                                                template.End);
                await _factory.Context.SaveChangesAsync();
            }
            catch (OperatorNotFoundException)
            {
                return BadRequest();
            }
            catch (DroneNotFoundException)
            {
                return BadRequest();
            }
            catch (LocationNotFoundException)
            {
                return BadRequest();
            }

            return flight;
        }
    }
}
