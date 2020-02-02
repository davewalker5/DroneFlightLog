using System;
using System.Collections.Generic;
using System.Linq;
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

            if (location == null)
            {
                string message = $"Location with ID {locationId} not found";
                throw new LocationNotFoundException(message);
            }

            return location;
        }

        /// <summary>
        /// Get all the current location details
        /// </summary>
        public IEnumerable<Location> GetLocations()
        {
            return _context.Locations;
        }

        /// <summary>
        /// Add a new location, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Location AddLocation(string name)
        {
            if (FindLocation(name) != null)
            {
                string message = $"Location {name} already exists";
                throw new LocationExistsException(message);
            }

            Location location = new Location { Name = name.CleanString() };
            _context.Locations.Add(location);
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
            return _context.Locations.FirstOrDefault(m => m.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
