using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DroneFlightLog.Data.Logic
{
    internal class MaintenanceRecordManager<T> : IMaintenanceRecordManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;

        internal MaintenanceRecordManager(IDroneFlightLogFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Add a new maintenance record
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="type"></param>
        /// <param name="completed"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public MaintenanceRecord AddMaintenanceRecord(int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes)
        {
            // These will throw exceptions if the corresponding entities do not exist
            _factory.Maintainers.GetMaintainer(maintainerId);
            _factory.Drones.GetDrone(droneId);

            MaintenanceRecord maintenanceRecord = new()
            {
                MaintainerId = maintainerId,
                DroneId = droneId,
                RecordType = type,
                DateCompleted = completed,
                Description = description,
                Notes = notes
            };

            _factory.Context.MaintenanceRecords.Add(maintenanceRecord);
            return maintenanceRecord;
        }

        /// <summary>
        /// Add a new maintenance record
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="type"></param>
        /// <param name="completed"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<MaintenanceRecord> AddMaintenanceRecordAsync(int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes)
        {
            // These will throw exceptions if the corresponding entities do not exist
            await _factory.Maintainers.GetMaintainerAsync(maintainerId);
            await _factory.Drones.GetDroneAsync(droneId);

            MaintenanceRecord maintenanceRecord = new()
            {
                MaintainerId = maintainerId,
                DroneId = droneId,
                RecordType = type,
                DateCompleted = completed,
                Description = description,
                Notes = notes
            };

            await _factory.Context.MaintenanceRecords.AddAsync(maintenanceRecord);
            return maintenanceRecord;
        }

        /// <summary>
        /// Get the maintenance record with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MaintenanceRecord GetMaintenanceRecord(int id)
        {
            MaintenanceRecord maintenanceRecord = _factory.Context
                                                          .MaintenanceRecords
                                                          .Include(m => m.Maintainer)
                                                          .Include(m => m.Drone)
                                                          .FirstOrDefault(m => m.Id == id);
            ThrowIfMaintenanceRecordNotFound(maintenanceRecord, id);
            return maintenanceRecord;
        }

        /// <summary>
        /// Get the maintenance record with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MaintenanceRecord> GetMaintenanceRecordAsync(int id)
        {
            MaintenanceRecord maintenanceRecord = await _factory.Context
                                                        .MaintenanceRecords
                                                        .Include(m => m.Maintainer)
                                                        .Include(m => m.Drone)
                                                        .FirstOrDefaultAsync(m => m.Id == id);
            ThrowIfMaintenanceRecordNotFound(maintenanceRecord, id);
            return maintenanceRecord;
        }

        /// <summary>
        /// Update the maintenance record with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="type"></param>
        /// <param name="completed"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public MaintenanceRecord UpdateMaintenanceRecord(int id, int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes)
        {
            // These will throw exceptions if the corresponding entities do not exist
            _factory.Maintainers.GetMaintainer(maintainerId);
            _factory.Drones.GetDrone(droneId);

            MaintenanceRecord maintenanceRecord = GetMaintenanceRecord(id);
            maintenanceRecord.MaintainerId = maintainerId;
            maintenanceRecord.DroneId = droneId;
            maintenanceRecord.RecordType = type;
            maintenanceRecord.DateCompleted = completed;
            maintenanceRecord.Description = description;
            maintenanceRecord.Notes = notes;

            return maintenanceRecord;
        }

        /// <summary>
        /// Update the maintenance record with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="type"></param>
        /// <param name="completed"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<MaintenanceRecord> UpdateMaintenanceRecordAsync(int id, int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes)
        {
            // These will throw exceptions if the corresponding entities do not exist
            await _factory.Maintainers.GetMaintainerAsync(maintainerId);
            await _factory.Drones.GetDroneAsync(droneId);

            MaintenanceRecord maintenanceRecord = await GetMaintenanceRecordAsync(id);
            maintenanceRecord.MaintainerId = maintainerId;
            maintenanceRecord.DroneId = droneId;
            maintenanceRecord.RecordType = type;
            maintenanceRecord.DateCompleted = completed;
            maintenanceRecord.Description = description;
            maintenanceRecord.Notes = notes;

            return maintenanceRecord;
        }

        /// <summary>
        /// Find maintenance records for a maintainer, drone and date range
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<MaintenanceRecord> FindMaintenanceRecords(int? maintainerId, int? droneId, DateTime? start, DateTime? end, int pageNumber, int pageSize)
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = _factory.Context
                                                                        .MaintenanceRecords
                                                                        .Include(m => m.Maintainer)
                                                                        .Include(m => m.Drone)
                                                                        .Where(m => ((maintainerId == null) || (maintainerId == m.MaintainerId)) &&
                                                                                    ((droneId == null) || (droneId == m.DroneId)) &&
                                                                                    ((start == null) || (m.DateCompleted >= start)) &&
                                                                                    ((end == null) || (m.DateCompleted <= end)))
                                                                        .OrderBy(m => m.DateCompleted)
                                                                        .Skip((pageNumber - 1) * pageSize)
                                                                        .Take(pageSize);

            return maintenanceRecords;
        }

        /// <summary>
        /// Find maintenance records for a maintainer, drone and date range
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MaintenanceRecord> FindMaintenanceRecordsAsync(int? maintainerId, int? droneId, DateTime? start, DateTime? end, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<MaintenanceRecord> maintenanceRecords = _factory.Context
                                                                             .MaintenanceRecords
                                                                             .Include(m => m.Maintainer)
                                                                             .Include(m => m.Drone)
                                                                             .Where(m => ((maintainerId == null) || (maintainerId == m.MaintainerId)) &&
                                                                                         ((droneId == null) || (droneId == m.DroneId)) &&
                                                                                         ((start == null) || (m.DateCompleted >= start)) &&
                                                                                         ((end == null) || (m.DateCompleted <= end)))
                                                                             .OrderBy(m => m.DateCompleted)
                                                                             .Skip((pageNumber - 1) * pageSize)
                                                                             .Take(pageSize)
                                                                             .AsAsyncEnumerable();
            return maintenanceRecords;
        }

        /// <summary>
        /// Throw an error if a maintenance record does not exist
        /// </summary>
        /// <param name="maintenanceRecord"></param>
        /// <param name="maintenanceRecordId"></param>
        [ExcludeFromCodeCoverage]
        private static void ThrowIfMaintenanceRecordNotFound(MaintenanceRecord maintenanceRecord, int maintenanceRecordId)
        {
            if (maintenanceRecord == null)
            {
                string message = $"Maintenance record with ID {maintenanceRecordId} not found";
                throw new MaintenanceRecordNotFoundException(message);
            }
        }
    }
}
