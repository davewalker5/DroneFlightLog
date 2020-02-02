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
            Model model = _factory.Context.Models.Include(m => m.Manufacturer).FirstOrDefault(a => a.Id == modelId);

            if (model == null)
            {
                string message = $"Model with ID {modelId} not found";
                throw new ModelNotFoundException(message);
            }

            return model;
        }

        /// <summary>
        /// Get all the current model details, optionally filtering by manufacturer
        /// </summary>
        /// <param name="manufacturerId"></param>
        public IEnumerable<Model> GetModels(int? manufacturerId)
        {
            IEnumerable<Model> models = (manufacturerId == null) ? _factory.Context.Models.Include(m => m.Manufacturer) : _factory.Context.Models.Include(m => m.Manufacturer).Where(m => m.ManufacturerId == manufacturerId);
            return models;
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

            if (FindModel(name, manufacturerId) != null)
            {
                string message = $"Model {name} for manufacturer with ID {manufacturerId} already exists";
                throw new ModelExistsException(message);
            }

            Model model = new Model { Name = name.CleanString(), ManufacturerId = manufacturerId };
            _factory.Context.Models.Add(model);
            return model;
        }

        /// <summary>
        /// Find a model given its details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        private Model FindModel(string name, int manufacturerId)
        {
            name = name.CleanString();
            return _factory.Context.Models.Include(m => m.Manufacturer).FirstOrDefault(m => m.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) &&
                                                                                            (m.ManufacturerId == manufacturerId));
        }
    }
}
