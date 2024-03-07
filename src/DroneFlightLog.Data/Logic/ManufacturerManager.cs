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
            ThrowIfManufacturerNotFound(manufacturer, manufacturerId);
            return manufacturer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Manufacturer> GetManufacturerAsync(int manufacturerId)
        {
            Manufacturer manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == manufacturerId);
            ThrowIfManufacturerNotFound(manufacturer, manufacturerId);
            return manufacturer;
        }

        /// <summary>
        /// Get all the current manufacturer details
        /// </summary>
        public IEnumerable<Manufacturer> GetManufacturers() =>
            _context.Manufacturers;

        /// <summary>
        /// Get all the current manufacturer details 
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerable<Manufacturer> GetManufacturersAsync() =>
            _context.Manufacturers.AsAsyncEnumerable();

        /// <summary>
        /// Add a new manufacturer, given their details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Manufacturer AddManufacturer(string name)
        {
            Manufacturer manufacturer = FindManufacturer(name);
            ThrowIfManufacturerFound(manufacturer, name);
            manufacturer = new Manufacturer { Name = name.CleanString() };
            _context.Manufacturers.Add(manufacturer);
            return manufacturer;
        }

        /// <summary>
        /// Add a new manufacturer, given their details
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> AddManufacturerAsync(string name)
        {
            Manufacturer manufacturer = await FindManufacturerAsync(name);
            ThrowIfManufacturerFound(manufacturer, name);
            manufacturer = new Manufacturer { Name = name.CleanString() };
            await _context.Manufacturers.AddAsync(manufacturer);
            return manufacturer;
        }

        /// <summary>
        /// Update a manufacturers name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Manufacturer UpdateManufacturer(int id, string name)
        {
            Manufacturer existing = FindManufacturer(name);
            ThrowIfManufacturerFound(existing, name);

            Manufacturer manufacturer = _context.Manufacturers.FirstOrDefault(m => m.Id == id);
            ThrowIfManufacturerNotFound(manufacturer, id);

            manufacturer.Name = name.CleanString();
            return manufacturer;
        }

        /// <summary>
        /// Update a manufacturers name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> UpdateManufacturerAsync(int id, string name)
        {
            Manufacturer existing = await FindManufacturerAsync(name);
            ThrowIfManufacturerFound(existing, name);

            Manufacturer manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == id);
            ThrowIfManufacturerNotFound(manufacturer, id);

            manufacturer.Name = name.CleanString();
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
            return _context.Manufacturers.FirstOrDefault(m => m.Name == name);
        }

        /// <summary>
        /// Find a manufacturer given their name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> FindManufacturerAsync(string name)
        {
            name = name.CleanString();
            return await _context.Manufacturers.FirstOrDefaultAsync(m => m.Name == name);
        }

        /// <summary>
        /// Throw an error if a manufacturer does not exist
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="manufacturerId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfManufacturerNotFound(Manufacturer manufacturer, int manufacturerId)
        {
            if (manufacturer == null)
            {
                string message = $"Manufacturer with ID {manufacturerId} not found";
                throw new ManufacturerNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an error if a manufacturer already exists
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="name"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfManufacturerFound(Manufacturer manufacturer, string name)
        {
            if (manufacturer != null)
            {
                string message = $"Manufacturer {name} already exists";
                throw new ManufacturerExistsException(message);
            }
        }
    }
}
