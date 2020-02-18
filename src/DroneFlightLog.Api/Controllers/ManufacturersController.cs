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
    public class ManufacturersController : Controller
    {
        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public ManufacturersController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Manufacturer>>> GetManufacturersAsync()
        {
            List<Manufacturer> manufacturers = await _factory.Manufacturers.GetManufacturersAsync().ToListAsync();

            if (!manufacturers.Any())
            {
                return NoContent();
            }

            return manufacturers;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Manufacturer>> GetManufacturerAsync(int id)
        {
            Manufacturer manufacturer;

            try
            {
                manufacturer = await _factory.Manufacturers.GetManufacturerAsync(id);
            }
            catch (ManufacturerNotFoundException)
            {
                return NotFound();
            }

            return manufacturer;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Manufacturer>> CreateManufacturerAsync([FromBody] string name)
        {
            Manufacturer manufacturer;

            try
            {
                manufacturer = await _factory.Manufacturers.AddManufacturerAsync(name);
                await _factory.Context.SaveChangesAsync();
            }
            catch (ManufacturerExistsException)
            {
                return BadRequest();
            }

            return manufacturer;
        }
    }
}
