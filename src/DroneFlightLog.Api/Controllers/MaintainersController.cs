using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DroneFlightLog.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class MaintainersController : Controller
    {
        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public MaintainersController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Maintainer>> GetMaintainerAsync(int id)
        {
            Maintainer maintainer;

            try
            {
                maintainer = await _factory.Maintainers.GetMaintainerAsync(id);
            }
            catch (MaintainerNotFoundException)
            {
                return NotFound();
            }

            return maintainer;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Maintainer>>> GetMaintainersAsync()
        {
            List<Maintainer> maintainers = await _factory.Maintainers.GetMaintainersAsync().ToListAsync();

            if (maintainers.Count == 0)
            {
                return NoContent();
            }

            return maintainers;
        }

        [HttpGet]
        [Route("{firstNames}/{surname}")]
        public async Task<ActionResult<Maintainer>> FindMaintainerAsync(string firstNames, string surname)
        {
            string decodedFirstNames = HttpUtility.UrlDecode(firstNames);
            string decodedSurname = HttpUtility.UrlDecode(surname);
            Maintainer maintainer = await _factory.Maintainers.FindMaintainerAsync(decodedFirstNames, decodedSurname);

            if (maintainer == null)
            {
                return NotFound();
            }

            return maintainer;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Maintainer>> UpdateMaintainerAsync([FromBody] Maintainer template)
        {
            Maintainer maintainer;

            try
            {
                maintainer = await _factory.Maintainers.UpdateMaintainerAsync(template.Id,
                                                                              template.FirstNames,
                                                                              template.Surname);
                await _factory.Context.SaveChangesAsync();
            }
            catch (MaintainerNotFoundException)
            {
                return NotFound();
            }

            return maintainer;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Maintainer>> CreateMaintainerAsync([FromBody] Maintainer template)
        {
            Maintainer maintainer;

            try
            {
                maintainer = await _factory.Maintainers.AddMaintainerAsync(template.FirstNames,
                                                                           template.Surname);
                await _factory.Context.SaveChangesAsync();
            }
            catch (MaintainerExistsException)
            {
                return BadRequest();
            }

            return maintainer;
        }
    }
}
