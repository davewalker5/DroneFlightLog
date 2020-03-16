using System;
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
    internal class FlightPropertyManager<T> : IFlightPropertyManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly T _context;

        internal FlightPropertyManager(T context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a new flight property definition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="isSingleInstance"></param>
        public FlightProperty AddProperty(string name, FlightPropertyDataType type, bool isSingleInstance)
        {
            FlightProperty property = _context.FlightProperties.FirstOrDefault(p => p.Name == name);
            ThrowIfPropertyFound(property, name);

            property = new FlightProperty
            {
                Name = name,
                DataType = type,
                IsSingleInstance = isSingleInstance
            };

            _context.FlightProperties.Add(property);
            return property;
        }

        /// <summary>
        /// Add a new flight property definition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="isSingleInstance"></param>
        /// <returns></returns>
        public async Task<FlightProperty> AddPropertyAsync(string name, FlightPropertyDataType type, bool isSingleInstance)
        {
            FlightProperty property = await _context.FlightProperties.FirstOrDefaultAsync(p => p.Name == name);
            ThrowIfPropertyFound(property, name);

            property = new FlightProperty
            {
                Name = name,
                DataType = type,
                IsSingleInstance = isSingleInstance
            };

            await _context.FlightProperties.AddAsync(property);
            return property;
        }

        /// <summary>
        /// Update an existing flight property definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FlightProperty UpdateProperty(int id, string name)
        {
            name = name.CleanString();
            FlightProperty existing = _context.FlightProperties.FirstOrDefault(p => p.Name == name);
            ThrowIfPropertyFound(existing, name);

            FlightProperty property = _context.FlightProperties.FirstOrDefault(p => p.Id == id);
            ThrowIfPropertyNotFound(property, id);

            // The only property that can be updated without running the risk of invalidating
            // existing data is the name
            property.Name = name;
            return property;
        }

        /// <summary>
        /// Update an existing flight property definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FlightProperty> UpdatePropertyAsync(int id, string name)
        {
            name = name.CleanString();
            FlightProperty existing = await _context.FlightProperties.FirstOrDefaultAsync(p => p.Name == name);
            ThrowIfPropertyFound(existing, name);

            FlightProperty property = await _context.FlightProperties.FirstOrDefaultAsync(p => p.Id == id);
            ThrowIfPropertyNotFound(property, id);

            // The only property that can be updated without running the risk of invalidating
            // existing data is the name
            property.Name = name;
            return property;
        }

        /// <summary>
        /// Add a value for a property to a flight
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="value"></param>
        public FlightPropertyValue AddPropertyValue(int flightId, int propertyId, object value)
        {
            FlightProperty definition = _context.FlightProperties.First(p => p.Id == propertyId);
            FlightPropertyValue propertyValue;

            if (definition.IsSingleInstance)
            {
                propertyValue = _context.FlightPropertyValues.FirstOrDefault(v => (v.FlightId == flightId) && (v.PropertyId == propertyId));
                ThrowIfPropertyValueFound(propertyValue, definition.Name, flightId);
            }

            propertyValue = CreatePropertyValue(definition, flightId, propertyId, value);
            _context.FlightPropertyValues.Add(propertyValue);
            return propertyValue;
        }

        /// <summary>
        /// Add a value for a property to a flight
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="value"></param>
        public async Task<FlightPropertyValue> AddPropertyValueAsync(int flightId, int propertyId, object value)
        {
            FlightProperty definition = await _context.FlightProperties.FirstAsync(p => p.Id == propertyId);
            FlightPropertyValue propertyValue;

            if (definition.IsSingleInstance)
            {
                propertyValue = await _context.FlightPropertyValues.FirstOrDefaultAsync(v => (v.FlightId == flightId) && (v.PropertyId == propertyId));
                ThrowIfPropertyValueFound(propertyValue, definition.Name, flightId);
            }

            propertyValue = CreatePropertyValue(definition, flightId, propertyId, value);
            await _context.FlightPropertyValues.AddAsync(propertyValue);
            return propertyValue;
        }

        /// <summary>
        /// Update an existing flight property definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public FlightPropertyValue UpdatePropertyValue(int id, object value)
        {
            FlightPropertyValue propertyValue = _context.FlightPropertyValues.FirstOrDefault(p => p.Id == id);
            ThrowIfPropertyValueNotFound(propertyValue, id);

            FlightProperty definition = _context.FlightProperties.First(p => p.Id == propertyValue.PropertyId);
            SetPropertyValue(definition, propertyValue, value);
            return propertyValue;
        }

        /// <summary>
        /// Update an existing flight property definition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> UpdatePropertyValueAsync(int id, object value)
        {
            FlightPropertyValue propertyValue = await _context.FlightPropertyValues.FirstOrDefaultAsync(p => p.Id == id);
            ThrowIfPropertyValueNotFound(propertyValue, id);

            FlightProperty definition = await _context.FlightProperties.FirstAsync(p => p.Id == propertyValue.PropertyId);
            SetPropertyValue(definition, propertyValue, value);
            return propertyValue;
        }

        /// <summary>
        /// Get all the current property definitions
        /// </summary>
        public IEnumerable<FlightProperty> GetProperties() =>
            _context.FlightProperties;

        /// <summary>
        /// Get all the current property definitions
        /// </summary>
        public IAsyncEnumerable<FlightProperty> GetPropertiesAsync() =>
            _context.FlightProperties.AsAsyncEnumerable();

        /// <summary>
        /// Return the property values for a specified flight
        /// </summary>
        /// <param name="flightId"></param>
        public IEnumerable<FlightPropertyValue> GetPropertyValues(int flightId) =>
            _context.FlightPropertyValues
                    .Include(v => v.Property)
                    .Where(v => v.FlightId == flightId);

        /// <summary>
        /// Return the property values for a specified flight
        /// </summary>
        /// <param name="flightId"></param>
        public IAsyncEnumerable<FlightPropertyValue> GetPropertyValuesAsync(int flightId) =>
            _context.FlightPropertyValues
                    .Include(v => v.Property)
                    .Where(v => v.FlightId == flightId)
                    .AsAsyncEnumerable();

        /// <summary>
        /// Return the specified flight property value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FlightPropertyValue GetPropertyValue(int id)
        {
            FlightPropertyValue value = _context.FlightPropertyValues
                                                .Include(v => v.Property)
                                                .FirstOrDefault(v => v.Id == id);
            ThrowIfPropertyValueNotFound(value, id);
            return value;
        }

        /// <summary>
        /// Return the specified flight property value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FlightPropertyValue> GetPropertyValueAsync(int id)
        {
            FlightPropertyValue value = await _context.FlightPropertyValues
                                                      .Include(v => v.Property)
                                                      .FirstOrDefaultAsync(v => v.Id == id);
            ThrowIfPropertyValueNotFound(value, id);
            return value;
        }

        /// <summary>
        /// Create a new property value
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="flightId"></param>
        /// <param name="propertyId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private FlightPropertyValue CreatePropertyValue(FlightProperty definition, int flightId, int propertyId, object value)
        {
            FlightPropertyValue propertyValue = null;
            switch (definition.DataType)
            {
                case FlightPropertyDataType.Date:
                    propertyValue = new FlightPropertyValue
                    {
                        FlightId = flightId,
                        PropertyId = propertyId
                    };
                    break;
                case FlightPropertyDataType.Number:
                    propertyValue = new FlightPropertyValue
                    {
                        FlightId = flightId,
                        PropertyId = propertyId
                    };
                    break;
                case FlightPropertyDataType.String:
                    propertyValue = new FlightPropertyValue
                    {
                        FlightId = flightId,
                        PropertyId = propertyId
                    };
                    break;
                default:
                    break;
            }

            SetPropertyValue(definition, propertyValue, value);
            return propertyValue;
        }

        /// <summary>
        /// Set the value field on an existing property value object
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="propertyValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private void SetPropertyValue(FlightProperty definition, FlightPropertyValue propertyValue, object value)
        {
            switch (definition.DataType)
            {
                case FlightPropertyDataType.Date:
                    propertyValue.DateValue = (DateTime)value;
                    break;
                case FlightPropertyDataType.Number:
                    propertyValue.NumberValue = (decimal)value;
                    break;
                case FlightPropertyDataType.String:
                    propertyValue.StringValue = (string)value;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Throw an error if a property already exists
        /// </summary>
        /// <param name="property"></param>
        /// <param name="name"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfPropertyFound(FlightProperty property, string name)
        {
            if (property != null)
            {
                string message = $"Property {name} already exists";
                throw new PropertyExistsException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a flight property does not exist
        /// </summary>
        /// <param name="property"></param>
        /// <param name="id"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfPropertyNotFound(FlightProperty property, int id)
        {
            if (property == null)
            {
                string message = $"Flight property with ID {id} not found";
                throw new PropertyNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an error if a single-instance property value already exists
        /// </summary>
        /// <param name="property"></param>
        /// <param name="name"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfPropertyValueFound(FlightPropertyValue propertyValue, string propertyName, int flightId)
        {
            if (propertyValue != null)
            {
                string message = $"Single instance property {propertyName} already has a value for flight Id {flightId}";
                throw new ValueExistsException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a flight property value does not exist
        /// </summary>
        /// <param name="value"></param>
        /// <param name="id"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfPropertyValueNotFound(FlightPropertyValue value, int id)
        {
            if (value == null)
            {
                string message = $"Flight property value with ID {id} not found";
                throw new PropertyValueNotFoundException(message);
            }
        }
    }
}
