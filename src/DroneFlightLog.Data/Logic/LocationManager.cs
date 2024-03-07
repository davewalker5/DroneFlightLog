using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Extensions;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Logic
{
    internal class LocationManager<T> : ILocationManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly T _context;

        internal LocationManager(T context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the drone with the specified Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public Location GetLocation(int locationId)
        {
            Location location = _context.Locations.FirstOrDefault(l => l.Id == locationId);
            ThrowIfLocationNotFound(location, locationId);
            return location;
        }

        /// <summary>
        /// Return the drone with the specified Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public async Task<Location> GetLocationAsync(int locationId)
        {
            Location location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
            ThrowIfLocationNotFound(location, locationId);
            return location;
        }

        /// <summary>
        /// Get all the current location details
        /// </summary>
        public IEnumerable<Location> GetLocations() =>
            _context.Locations;

        /// <summary>
        /// Get all the current location details
        /// </summary>
        public IAsyncEnumerable<Location> GetLocationsAsync() =>
            _context.Locations.AsAsyncEnumerable();

        /// <summary>
        /// Add a new location, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Location AddLocation(string name)
        {
            Location location = FindLocation(name);
            ThrowIfLocationFound(location, name);
            location = new Location { Name = name.CleanString() };
            _context.Locations.Add(location);
            return location;
        }

        /// <summary>
        /// Add a new location, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddLocationAsync(string name)
        {
            Location location = await FindLocationAsync(name);
            ThrowIfLocationFound(location, name);
            location = new Location { Name = name.CleanString() };
            await _context.Locations.AddAsync(location);
            return location;
        }

        /// <summary>
        /// Update a location name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Location UpdateLocation(int id, string name)
        {
            Location existing = FindLocation(name);
            ThrowIfLocationFound(existing, name);

            Location location = _context.Locations.FirstOrDefault(m => m.Id == id);
            ThrowIfLocationNotFound(location, id);

            location.Name = name.CleanString();
            return location;
        }

        /// <summary>
        /// Update a location name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> UpdateLocationAsync(int id, string name)
        {
            Location existing = await FindLocationAsync(name);
            ThrowIfLocationFound(existing, name);

            Location location = await _context.Locations.FirstOrDefaultAsync(m => m.Id == id);
            ThrowIfLocationNotFound(location, id);

            location.Name = name.CleanString();
            return location;
        }

        /// <summary>
        /// Find a location given its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Location FindLocation(string name)
        {
            name = name.CleanString();
            return _context.Locations.FirstOrDefault(m => m.Name == name);
        }

        /// <summary>
        /// Find a location given its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> FindLocationAsync(string name)
        {
            name = name.CleanString();
            return await _context.Locations.FirstOrDefaultAsync(m => m.Name == name);
        }

        /// <summary>
        /// Throw an exception if a location is not found
        /// </summary>
        /// <param name="location"></param>
        /// <param name="locationId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfLocationNotFound(Location location, int locationId)
        {
            if (location == null)
            {
                string message = $"Location with ID {locationId} not found";
                throw new LocationNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a location exists
        /// </summary>
        /// <param name="location"></param>
        /// <param name="name"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfLocationFound(Location location, string name)
        {
            if (location != null)
            {
                string message = $"Location {name} already exists";
                throw new LocationExistsException(message);
            }
        }
    }
}
