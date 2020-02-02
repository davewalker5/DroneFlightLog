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
    internal class ManufacturerManager<T> : IManufacturerManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly T _context;

        internal ManufacturerManager(T context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the manufacturer with the specified Id
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public Manufacturer GetManufacturer(int manufacturerId)
        {
            Manufacturer manufacturer = _context.Manufacturers.FirstOrDefault(m => m.Id == manufacturerId);

            if (manufacturer == null)
            {
                string message = $"Manufacturer with ID {manufacturerId} not found";
                throw new ManufacturerNotFoundException(message);
            }

            return manufacturer;
        }

        /// <summary>
        /// Get all the current manufacturer details
        /// </summary>
        public IEnumerable<Manufacturer> GetManufacturers()
        {
            return _context.Manufacturers;
        }

        /// <summary>
        /// Add a new manufacturer, given their details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Manufacturer AddManufacturer(string name)
        {
            if (FindManufacturer(name) != null)
            {
                string message = $"Manufacturer {name} already exists";
                throw new ManufacturerExistsException(message);
            }

            Manufacturer manufacturer = new Manufacturer { Name = name.CleanString() };
            _context.Manufacturers.Add(manufacturer);
            return manufacturer;
        }

        /// <summary>
        /// Find a manufacturer given their name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Manufacturer FindManufacturer(string name)
        {
            name = name.CleanString();
            return _context.Manufacturers.FirstOrDefault(m => m.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
