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
    internal class DroneManager<T> : IDroneManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;

        internal DroneManager(IDroneFlightLogFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return the drone with the specified Id
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public Drone GetDrone(int droneId)
        {
            Drone drone = _factory.Context.Drones
                                          .Include(d => d.Model)
                                          .ThenInclude(m => m.Manufacturer)
                                          .FirstOrDefault(d => d.Id == droneId);
            ThrowIfDroneNotFound(drone, droneId);
            return drone;
        }

        /// <summary>
        /// Return the drone with the specified Id
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public async Task<Drone> GetDroneAsync(int droneId)
        {
            Drone drone = await _factory.Context.Drones
                                                .Include(d => d.Model)
                                                .ThenInclude(m => m.Manufacturer)
                                                .FirstOrDefaultAsync(d => d.Id == droneId);
            ThrowIfDroneNotFound(drone, droneId);
            return drone;
        }

        /// <summary>
        /// Get all the current drone details, optionally filtering by model
        /// </summary>
        /// <param name="modelId"></param>
        public IEnumerable<Drone> GetDrones(int? modelId)
        {
            IEnumerable<Drone> drones;

            if (modelId == null)
            {
                drones = _factory.Context.Drones
                                         .Include(d => d.Model)
                                         .ThenInclude(m => m.Manufacturer);
            }
            else
            {
                drones = _factory.Context.Drones
                                         .Include(d => d.Model)
                                         .ThenInclude(m => m.Manufacturer)
                                         .Where(m => m.ModelId == modelId);
            }

            return drones;
        }

        /// <summary>
        /// Get all the current drone details, optionally filtering by model
        /// </summary>
        /// <param name="modelId"></param>
        public IAsyncEnumerable<Drone> GetDronesAsync(int? modelId)
        {
            IAsyncEnumerable<Drone> drones;

            if (modelId == null)
            {
                drones = _factory.Context.Drones
                                         .Include(d => d.Model)
                                         .ThenInclude(m => m.Manufacturer)
                                         .AsAsyncEnumerable();
            }
            else
            {
                drones = _factory.Context.Drones
                                         .Include(d => d.Model)
                                         .ThenInclude(m => m.Manufacturer)
                                         .Where(m => m.ModelId == modelId)
                                         .AsAsyncEnumerable();
            }

            return drones;
        }

        /// <summary>
        /// Add a new drone, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialNumber"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public Drone AddDrone(string name, string serialNumber, int modelId)
        {
            // This will throw an exception if the model does not exist
            _factory.Models.GetModel(modelId);

            Drone drone = FindDrone(serialNumber, modelId);
            ThrowIfDroneFound(drone, serialNumber, modelId);

            drone = new Drone
            {
                Name = name.CleanString(),
                SerialNumber = serialNumber.CleanString(),
                ModelId = modelId
            };

            _factory.Context.Drones.Add(drone);
            return drone;
        }

        /// <summary>
        /// Add a new drone, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialNumber"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Drone> AddDroneAsync(string name, string serialNumber, int modelId)
        {
            // This will throw an exception if the model does not exist
            await _factory.Models.GetModelAsync(modelId);

            Drone drone = await FindDroneAsync(serialNumber, modelId);
            ThrowIfDroneFound(drone, serialNumber, modelId);

            drone = new Drone
            {
                Name = name.CleanString(),
                SerialNumber = serialNumber.CleanString(),
                ModelId = modelId
            };

            await _factory.Context.Drones.AddAsync(drone);
            return drone;
        }

        /// <summary>
        /// Find a model given its distinguishing details
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public Drone FindDrone(string serialNumber, int modelId)
        {
            serialNumber = serialNumber.CleanString();
            return _factory.Context.Drones
                                   .Include(d => d.Model)
                                   .ThenInclude(m => m.Manufacturer)
                                   .FirstOrDefault(m => (m.SerialNumber == serialNumber) &&
                                                        (m.ModelId == modelId));
        }

        /// <summary>
        /// Find a model given its distinguishing details
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Drone> FindDroneAsync(string serialNumber, int modelId)
        {
            serialNumber = serialNumber.CleanString();
            return await _factory.Context.Drones
                                         .Include(d => d.Model)
                                         .ThenInclude(m => m.Manufacturer)
                                         .FirstOrDefaultAsync(m => (m.SerialNumber == serialNumber) &&
                                                                   (m.ModelId == modelId));
        }

        /// <summary>
        /// Throw an error if a drone does not exist
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="droneId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfDroneNotFound(Drone drone, int droneId)
        {
            if (drone == null)
            {
                string message = $"Drone with ID {droneId} not found";
                throw new DroneNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a drone exists
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="serialNumber"></param>
        /// <param name="modelId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfDroneFound(Drone drone, string serialNumber, int modelId)
        {
            if (drone != null)
            {
                string message = $"Drone with serial number {serialNumber} for model with ID {modelId} already exists";
                throw new DroneExistsException(message);
            }
        }
    }
}
