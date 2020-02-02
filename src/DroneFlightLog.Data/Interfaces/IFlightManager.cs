using System;
using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IFlightManager
    {
        Flight AddFlight(int operatorId, int droneId, int locationId, DateTime start, DateTime end);
        IEnumerable<Flight> FindFlights(int? operatorId, int? droneId, int? locationId, DateTime? start, DateTime? end, int pageNumber, int pageSize);
    }
}