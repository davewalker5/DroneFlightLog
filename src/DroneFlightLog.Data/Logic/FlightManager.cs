using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Logic
{
    internal class FlightManager<T> : IFlightManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;

        internal FlightManager(IDroneFlightLogFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Add a flight, given its details
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Flight AddFlight(int operatorId, int droneId, int locationId, DateTime start, DateTime end)
        {
            // These calls will throw an exception if the entity with the specified ID doesn't exist
            _factory.Operators.GetOperator(operatorId);
            _factory.Drones.GetDrone(droneId);
            _factory.Locations.GetLocation(locationId);

            Flight flight = new Flight
            {
                OperatorId = operatorId,
                DroneId = droneId,
                LocationId = locationId,
                Start = start,
                End = end
            };

            _factory.Context.Flights.Add(flight);
            return flight;
        }

        /// <summary>
        /// Add a flight, given its details
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<Flight> AddFlightAsync(int operatorId, int droneId, int locationId, DateTime start, DateTime end)
        {
            // These calls will throw an exception if the entity with the specified ID doesn't exist
            await _factory.Operators.GetOperatorAsync(operatorId);
            await _factory.Drones.GetDroneAsync(droneId);
            await _factory.Locations.GetLocationAsync(locationId);

            Flight flight = new Flight
            {
                OperatorId = operatorId,
                DroneId = droneId,
                LocationId = locationId,
                Start = start,
                End = end
            };

            await _factory.Context.Flights.AddAsync(flight);
            return flight;
        }

        /// <summary>
        /// Return a single flight given its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Flight GetFlight(int id)
        {
            Flight flight = _factory.Context.Flights.FirstOrDefault(f => f.Id == id);
            ThrowIfFlightNotFound(flight, id);
            return flight;
        }

        /// <summary>
        /// Return a single flight given its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightAsync(int id)
        {
            Flight flight = await _factory.Context.Flights.FirstOrDefaultAsync(f => f.Id == id);
            ThrowIfFlightNotFound(flight, id);
            return flight;
        }

        /// <summary>
        /// Find flights matching the specified filtering criteria and return the specified
        /// page of results
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Flight> FindFlights(int? operatorId, int? droneId, int? locationId, DateTime? start, DateTime? end, int pageNumber, int pageSize)
        {
            IEnumerable<Flight> flights = _factory.Context.Flights
                                                          .Include(f => f.Drone)
                                                            .ThenInclude(d => d.Model)
                                                                .ThenInclude(m => m.Manufacturer)
                                                          .Include(f => f.Location)
                                                          .Include(f => f.Operator)
                                                            .ThenInclude(o => o.Address)
                                                          .Where(f =>   ((operatorId == null) || (operatorId == f.OperatorId)) &&
                                                                        ((droneId == null) || (droneId == f.DroneId)) &&
                                                                        ((locationId == null) || (locationId == f.LocationId)) &&
                                                                        ((start == null) || (f.Start >= start)) &&
                                                                        ((end == null) || (f.End <= end)))
                                                          .Skip((pageNumber - 1) * pageSize)
                                                          .Take(pageSize);

            return flights;
        }

        /// <summary>
        /// Find flights matching the specified filtering criteria and return the specified
        /// page of results
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="droneId"></param>
        /// <param name="locationId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Flight> FindFlightsAsync(int? operatorId, int? droneId, int? locationId, DateTime? start, DateTime? end, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Flight> flights = _factory.Context.Flights
                                                               .Include(f => f.Drone)
                                                                 .ThenInclude(d => d.Model)
                                                                     .ThenInclude(m => m.Manufacturer)
                                                               .Include(f => f.Location)
                                                               .Include(f => f.Operator)
                                                                 .ThenInclude(o => o.Address)
                                                               .Where(f => ((operatorId == null) || (operatorId == f.OperatorId)) &&
                                                                           ((droneId == null) || (droneId == f.DroneId)) &&
                                                                           ((locationId == null) || (locationId == f.LocationId)) &&
                                                                           ((start == null) || (f.Start >= start)) &&
                                                                           ((end == null) || (f.End <= end)))
                                                               .Skip((pageNumber - 1) * pageSize)
                                                               .Take(pageSize)
                                                               .AsAsyncEnumerable();

            return flights;
        }

        /// <summary>
        /// Throw an error if a flight does not exist
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="flightId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfFlightNotFound(Flight flight, int flightId)
        {
            if (flight == null)
            {
                string message = $"Flight with ID {flightId} not found";
                throw new FlightNotFoundException(message);
            }
        }
    }
}
