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
    public class AddressesController : Controller
    {
        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public AddressesController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Address>> GetAddressAsync(int id)
        {
            Address address;

            try
            {
                address = await _factory.Addresses.GetAddressAsync(id);
            }
            catch (AddressNotFoundException)
            {
                return NotFound();
            }

            return address;
        }

        [HttpGet]
        [Route("{number}/{postcode}/{country}")]
        public async Task<ActionResult<Address>> FindAddressAsync(string number, string postcode, string country)
        {
            string decodedNumber = HttpUtility.UrlDecode(number);
            string decodedPostcode = HttpUtility.UrlDecode(postcode);
            string decodedCountry = HttpUtility.UrlDecode(country);
            Address address = await _factory.Addresses.FindAddressAsync(decodedNumber, decodedPostcode, decodedCountry);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Address>> CreateModelAsync([FromBody] Address template)
        {
            Address address;

            try
            {
                address = await _factory.Addresses.AddAddressAsync(template.Number, template.Street, template.Town, template.County, template.Postcode, template.Country);
                await _factory.Context.SaveChangesAsync();
            }
            catch (AddressExistsException)
            {
                return BadRequest();
            }

            return address;
        }
    }
}
