using DroneFlightLog.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IMaintenanceRecordManager
    {
        MaintenanceRecord AddMaintenanceRecord(int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes);
        Task<MaintenanceRecord> AddMaintenanceRecordAsync(int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes);
        IEnumerable<MaintenanceRecord> FindMaintenanceRecords(int? maintainerId, int? droneId, DateTime? start, DateTime? end, int pageNumber, int pageSize);
        IAsyncEnumerable<MaintenanceRecord> FindMaintenanceRecordsAsync(int? maintainerId, int? droneId, DateTime? start, DateTime? end, int pageNumber, int pageSize);
        MaintenanceRecord GetMaintenanceRecord(int id);
        Task<MaintenanceRecord> GetMaintenanceRecordAsync(int id);
        MaintenanceRecord UpdateMaintenanceRecord(int id, int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes);
        Task<MaintenanceRecord> UpdateMaintenanceRecordAsync(int id, int maintainerId, int droneId, MaintenanceRecordType type, DateTime completed, string description, string notes);
    }
}