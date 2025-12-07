using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class MaintenanceRecordsController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;

        public MaintenanceRecordsController(IDroneFlightLogFactory<DroneFlightLogDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MaintenanceRecord>> GetMaintenanceRecordAsync(int id)
        {
            MaintenanceRecord maintenanceRecord;

            try
            {
                maintenanceRecord = await _factory.MaintenanceRecords.GetMaintenanceRecordAsync(id);
            }
            catch (MaintenanceRecordNotFoundException)
            {
                return NotFound();
            }

            return maintenanceRecord;
        }

        [HttpGet]
        [Route("{droneId}/{start}/{end}/{page}/{pageSize}")]
        public async Task<ActionResult<List<MaintenanceRecord>>> FindMaintenanceRecordsAsync(int droneId, string start, string end, int page, int pageSize)
        {
            DateTime recordsFrom = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime recordsTo = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            List<MaintenanceRecord> maintenanceRecords = await _factory.MaintenanceRecords
                                                                       .FindMaintenanceRecordsAsync(
                                                                            null,
                                                                            droneId,
                                                                            recordsFrom,
                                                                            recordsTo,
                                                                            page,
                                                                            pageSize)
                                                                       .ToListAsync();

            if (maintenanceRecords.Count == 0)
            {
                return NoContent();
            }

            return maintenanceRecords;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<MaintenanceRecord>> UpdateMaintenanceRecordAsync([FromBody] MaintenanceRecord template)
        {
            MaintenanceRecord maintenanceRecord;

            try
            {
                maintenanceRecord = await _factory.MaintenanceRecords.UpdateMaintenanceRecordAsync(
                                                                            template.Id,
                                                                            template.MaintainerId,
                                                                            template.DroneId,
                                                                            template.RecordType,
                                                                            template.DateCompleted,
                                                                            template.Description,
                                                                            template.Notes);
                await _factory.Context.SaveChangesAsync();
            }
            catch (MaintainerNotFoundException)
            {
                return BadRequest();
            }
            catch (DroneNotFoundException)
            {
                return BadRequest();
            }
            catch (MaintenanceRecordNotFoundException)
            {
                return NotFound();
            }

            return maintenanceRecord;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<MaintenanceRecord>> CreateMaintenanceRecordAsync([FromBody] MaintenanceRecord template)
        {
            MaintenanceRecord maintainer;

            try
            {
                maintainer = await _factory.MaintenanceRecords.AddMaintenanceRecordAsync(
                                                                            template.MaintainerId,
                                                                            template.DroneId,
                                                                            template.RecordType,
                                                                            template.DateCompleted,
                                                                            template.Description,
                                                                            template.Notes);        
                await _factory.Context.SaveChangesAsync();
            }
            catch (MaintainerNotFoundException)
            {
                return BadRequest();
            }
            catch (DroneNotFoundException)
            {
                return BadRequest();
            }

            return maintainer;
        }
    }
}
