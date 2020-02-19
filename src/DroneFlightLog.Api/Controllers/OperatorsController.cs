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
    public class OperatorsController : Controller
    {
        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public OperatorsController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Operator>> GetOperatorAsync(int id)
        {
            Operator op;

            try
            {
                op = await _factory.Operators.GetOperatorAsync(id);
            }
            catch (LocationNotFoundException)
            {
                return NotFound();
            }

            return op;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Operator>>> GetOperatorsAsync()
        {
            List<Operator> operators = await _factory.Operators.GetOperatorsAsync(null).ToListAsync();

            if (!operators.Any())
            {
                return NoContent();
            }

            return operators;
        }

        [HttpGet]
        [Route("address/{addressId}")]
        public async Task<ActionResult<List<Operator>>> GetOperatorsForAddressAsync(int addressId)
        {
            List<Operator> operators;

            try
            {
                operators = await _factory.Operators.GetOperatorsAsync(addressId).ToListAsync();
                if (!operators.Any())
                {
                    return NoContent();
                }
            }
            catch (AddressNotFoundException)
            {
                return NotFound();
            }

            return operators;
        }

        [HttpGet]
        [Route("{firstNames}/{surname}/{addressId}")]
        public async Task<ActionResult<Operator>> FindOperatorAsync(string firstNames, string surname, int addressId)
        {
            string decodedFirstNames = HttpUtility.UrlDecode(firstNames);
            string decodedSurname = HttpUtility.UrlDecode(surname);
            Operator op = await _factory.Operators.FindOperatorAsync(decodedFirstNames, decodedSurname, addressId);

            if (op == null)
            {
                return NotFound();
            }

            return op;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> SetOperatorAddressAsync(int id, [FromBody] string addressId)
        {
            try
            {
                await _factory.Operators.SetOperatorAddressAsync(id, int.Parse(addressId));
                await _factory.Context.SaveChangesAsync();
            }
            catch (AddressNotFoundException)
            {
                return BadRequest();
            }
            catch (OperatorNotFoundException)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Operator>> CreateOperatorAsync([FromBody] Operator template)
        {
            Operator op;

            try
            {
                op = await _factory.Operators.AddOperatorAsync(template.FirstNames,
                                                               template.Surname,
                                                               template.DoB,
                                                               template.FlyerNumber,
                                                               template.OperatorNumber,
                                                               template.AddressId);
                await _factory.Context.SaveChangesAsync();
            }
            catch (AddressNotFoundException)
            {
                return BadRequest();
            }
            catch (OperatorExistsException)
            {
                return BadRequest();
            }

            return op;
        }
    }
}
