using System;
using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
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
            if (_context.FlightProperties.Any(p => p.Name == name))
            {
                string message = $"Property {name} already exists";
                throw new PropertyExistsException(message);
            }

            FlightProperty property = new FlightProperty
            {
                Name = name,
                DataType = type,
                IsSingleInstance = isSingleInstance
            };

            _context.FlightProperties.Add(property);
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
            FlightPropertyValue propertyValue = null;

            if (definition.IsSingleInstance && _context.FlightPropertyValues.Any(v => (v.FlightId == flightId) && (v.PropertyId == propertyId)))
            {
                string message = $"Single instance property {definition.Name} already has a value for flight Id {flightId}";
                throw new ValueExistsException(message);
            }

            switch (definition.DataType)
            {
                case FlightPropertyDataType.Date:
                    propertyValue = new FlightPropertyValue
                    {
                        FlightId = flightId,
                        PropertyId = propertyId,
                        DateValue = (DateTime)value
                    };
                    break;
                case FlightPropertyDataType.Number:
                    propertyValue = new FlightPropertyValue
                    {
                        FlightId = flightId,
                        PropertyId = propertyId,
                        NumberValue = (decimal)value
                    };
                    break;
                case FlightPropertyDataType.String:
                    propertyValue = new FlightPropertyValue
                    {
                        FlightId = flightId,
                        PropertyId = propertyId,
                        StringValue = (string)value
                    };
                    break;
                default:
                    break;
            }

            _context.FlightPropertyValues.Add(propertyValue);
            return propertyValue;
        }

        /// <summary>
        /// Get all the current property definitions
        /// </summary>
        public IEnumerable<FlightProperty> GetProperties()
        {
            return _context.FlightProperties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flightId"></param>
        public IEnumerable<FlightPropertyValue> GetPropertyValues(int flightId)
        {
            IEnumerable<FlightPropertyValue> values = _context.FlightPropertyValues
                                                              .Include(v => v.Property)
                                                              .Where(v => v.FlightId == flightId);
            return values;
        }
    }
}
