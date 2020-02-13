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
    internal class ModelManager<T> : IModelManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;

        internal ModelManager(IDroneFlightLogFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the model with the specified  ID
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public Model GetModel(int modelId)
        {
            Model model = _factory.Context.Models
                                          .Include(m => m.Manufacturer)
                                          .FirstOrDefault(a => a.Id == modelId);
            ThrowIfModelNotFound(model, modelId);
            return model;
        }

        /// <summary>
        /// Get the model with the specified  ID
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Model> GetModelAsync(int modelId)
        {
            Model model = await _factory.Context.Models
                                                .Include(m => m.Manufacturer)
                                                .FirstOrDefaultAsync(a => a.Id == modelId);
            ThrowIfModelNotFound(model, modelId);
            return model;
        }

        /// <summary>
        /// Get all the current model details, optionally filtering by manufacturer
        /// </summary>
        /// <param name="manufacturerId"></param>
        public IEnumerable<Model> GetModels(int? manufacturerId)
        {
            return (manufacturerId == null) ? _factory.Context.Models.Include(m => m.Manufacturer) :
                                              _factory.Context.Models.Include(m => m.Manufacturer)
                                                                     .Where(m => m.ManufacturerId == manufacturerId);
        }

        /// <summary>
        /// Get all the current model details, optionally filtering by manufacturer
        /// </summary>
        /// <param name="manufacturerId"></param>
        public IAsyncEnumerable<Model> GetModelsAsync(int? manufacturerId)
        {
            return (manufacturerId == null) ? _factory.Context.Models.Include(m => m.Manufacturer)
                                                                     .AsAsyncEnumerable() :
                                              _factory.Context.Models.Include(m => m.Manufacturer)
                                                                     .Where(m => m.ManufacturerId == manufacturerId)
                                                                     .AsAsyncEnumerable();
        }

        /// <summary>
        /// Add a new model, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public Model AddModel(string name, int manufacturerId)
        {
            // This will throw an exception if the manufacturer does not exist
            _factory.Manufacturers.GetManufacturer(manufacturerId);
            Model model = FindModel(name, manufacturerId);
            ThrowIfModelFound(model, name, manufacturerId);
            model = new Model { Name = name.CleanString(), ManufacturerId = manufacturerId };
            _factory.Context.Models.Add(model);
            return model;
        }

        /// <summary>
        /// Add a new model, given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddModelAsync(string name, int manufacturerId)
        {
            // This will throw an exception if the manufacturer does not exist
            await _factory.Manufacturers.GetManufacturerAsync(manufacturerId);
            Model model = await FindModelAsync(name, manufacturerId);
            ThrowIfModelFound(model, name, manufacturerId);
            model = new Model { Name = name.CleanString(), ManufacturerId = manufacturerId };
            await _factory.Context.Models.AddAsync(model);
            return model;
        }

        /// <summary>
        /// Find a model given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private Model FindModel(string name, int manufacturerId)
        {
            name = name.CleanString();
            return _factory.Context.Models
                                   .Include(m => m.Manufacturer)
                                   .FirstOrDefault(m => (m.Name == name) &&
                                                        (m.ManufacturerId == manufacturerId));
        }

        /// <summary>
        /// Find a model given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private async Task<Model> FindModelAsync(string name, int manufacturerId)
        {
            name = name.CleanString();
            return await _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .FirstOrDefaultAsync(m => (m.Name == name) &&
                                                                   (m.ManufacturerId == manufacturerId));
        }

        /// <summary>
        /// Throw an error if a model does not exist
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfModelNotFound(Model model, int modelId)
        {
            if (model == null)
            {
                string message = $"Model with ID {modelId} not found";
                throw new ModelNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an error if a model already exists
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfModelFound(Model model, string name, int manufacturerId)
        {
            if (model != null)
            {
                string message = $"Model {name} for manufacturer with ID {manufacturerId} already exists";
                throw new ModelExistsException(message);
            }
        }
    }
}
