using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LocationsController : Controller
    {
        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public LocationsController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Location>>> GetLocationsAsync()
        {
            List<Location> locations = await _factory.Locations.GetLocationsAsync().ToListAsync();

            if (!locations.Any())
            {
                return NoContent();
            }

            return locations;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Location>> GetLocationAsync(int id)
        {
            Location location;

            try
            {
                location = await _factory.Locations.GetLocationAsync(id);
            }
            catch (LocationNotFoundException)
            {
                return NotFound();
            }

            return location;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Location>> UpdateLocationAsync(int id, [FromBody] string name)
        {
            Location location;

            try
            {
                location = await _factory.Locations.UpdateLocationAsync(id, name);
                await _factory.Context.SaveChangesAsync();
            }
            catch (LocationExistsException)
            {
                return BadRequest();
            }
            catch (LocationNotFoundException)
            {
                return NotFound();
            }

            return location;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Location>> CreateLocationAsync([FromBody] string name)
        {
            Location location;

            try
            {
                location = await _factory.Locations.AddLocationAsync(name);
                await _factory.Context.SaveChangesAsync();
            }
            catch (LocationExistsException)
            {
                return BadRequest();
            }

            return location;
        }
    }
}
