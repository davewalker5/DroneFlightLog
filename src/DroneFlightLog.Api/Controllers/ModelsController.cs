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
    public class ModelsController : Controller
    {
        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public ModelsController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Model>>> GetModelsAsync()
        {
            List<Model> models = await _factory.Models.GetModelsAsync(null).ToListAsync();

            if (!models.Any())
            {
                return NoContent();
            }

            return models;
        }

        [HttpGet]
        [Route("manufacturer/{manufacturerId}")]
        public async Task<ActionResult<List<Model>>> GetModelsForManufacturerAsync(int manufacturerId)
        {
            List<Model> models = await _factory.Models.GetModelsAsync(manufacturerId).ToListAsync();

            if (!models.Any())
            {
                return NoContent();
            }

            return models;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Model>> GetModelAsync(int id)
        {
            Model model;

            try
            {
                model = await _factory.Models.GetModelAsync(id);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }

            return model;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Model>> UpdateModelAsync([FromBody] Model template)
        {
            Model model;

            try
            {
                model = await _factory.Models.UpdateModelAsync(template.Id, template.Name, template.ManufacturerId);
                await _factory.Context.SaveChangesAsync();
            }
            catch (ModelExistsException)
            {
                return BadRequest();
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Model>> CreateModelAsync([FromBody] Model template)
        {
            Model model;

            try
            {
                model = await _factory.Models.AddModelAsync(template.Name, template.ManufacturerId);
                await _factory.Context.SaveChangesAsync();
            }
            catch (ManufacturerNotFoundException)
            {
                return BadRequest();
            }
            catch (ModelExistsException)
            {
                return BadRequest();
            }

            return model;
        }
    }
}
