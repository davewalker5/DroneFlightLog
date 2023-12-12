using System;
using System.Collections.Generic;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Importer.Logic
{
    public class FlightWriter<T> where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;

        public FlightWriter(IDroneFlightLogFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Save the data for the specified collection of flights to the database
        /// </summary>
        /// <param name="flights"></param>
        public void Save(IList<Flight> flights)
        {
            foreach (Flight flight in flights)
            {
                Save(flight);
            }
        }

        /// <summary>
        /// Save the data in the specified flight object to the database
        /// </summary>
        /// <param name="flight"></param>
        private void Save(Flight flight)
        {
            // Save the flight itself, to get the flight ID
            Flight saved = _factory.Flights.AddFlight(flight.OperatorId,
                                                      flight.DroneId,
                                                      flight.LocationId,
                                                      flight.Start,
                                                      flight.End);
            _factory.Context.SaveChanges();

            // Save the flight properties for this flight
            foreach (FlightPropertyValue value in flight.Properties)
            {
                switch (value.Property.DataType)
                {
                    case FlightPropertyDataType.Date:
                        _factory.Properties.AddPropertyValue(saved.Id, value.PropertyId, value.DateValue);
                        break;
                    case FlightPropertyDataType.Number:
                        _factory.Properties.AddPropertyValue(saved.Id, value.PropertyId, value.NumberValue);
                        break;
                    case FlightPropertyDataType.String:
                        _factory.Properties.AddPropertyValue(saved.Id, value.PropertyId, value.StringValue);
                        break;
                    default:
                        break;
                }
            }

            // Commit the changes to the property values
            _factory.Context.SaveChanges();
        }
    }
}
