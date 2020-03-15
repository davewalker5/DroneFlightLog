using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IFlightManager
    {
        Flight AddFlight(int operatorId, int droneId, int locationId, DateTime start, DateTime end);
        Task<Flight> AddFlightAsync(int operatorId, int droneId, int locationId, DateTime start, DateTime end);
        IEnumerable<Flight> FindFlights(int? operatorId, int? droneId, int? locationId, DateTime? start, DateTime? end, int pageNumber, int pageSize);
        IAsyncEnumerable<Flight> FindFlightsAsync(int? operatorId, int? droneId, int? locationId, DateTime? start, DateTime? end, int pageNumber, int pageSize);
        Flight GetFlight(int id);
        Task<Flight> GetFlightAsync(int id);
    }
}