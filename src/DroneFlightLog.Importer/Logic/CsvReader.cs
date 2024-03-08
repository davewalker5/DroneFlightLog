using System;
using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Importer.Entities;
using DroneFlightLog.Importer.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace DroneFlightLog.Importer.Logic
{
    public class CsvReader<T> where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;
        private readonly AppSettings _settings;
        private List<CsvField> _fields;
        private IEnumerable<Drone> _drones;
        private IEnumerable<Location> _locations;
        private IEnumerable<Operator> _operators;

        public CsvReaderError LastError { get; private set; }

        public CsvReader(AppSettings settings, IDroneFlightLogFactory<T> factory)
        {
            _settings = settings;
            _factory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public IList<Flight> Read(string fileName)
        {
            // Retrieve the list of drones, locations and operators as these are
            // going to be used when processing evey record in the file
            _drones = _factory.Drones.GetDrones(null);
            _locations = _factory.Locations.GetLocations();
            _operators = _factory.Operators.GetOperators(null);

            // Create a new parser and set its properties
            TextFieldParser parser = new TextFieldParser(fileName);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            // Read and parse the headers to get the field list
            string[] headers = parser.ReadFields();
            ParseHeaders(headers);

            // Read to EOF on the file
            int count = 0;
            List<Flight> flights = new List<Flight>();
            while (!parser.EndOfData)
            {
                // Read the next record and create a new Flight object, with
                // property values, from the fields read in
                string[] record = parser.ReadFields();
                count++;
                Flight flight = CreateFlightFromRecord(count, record);
                flights.Add(flight);
            }

            return flights;
        }

        /// <summary>
        /// Parse the header field collection to create a set of fields that allow
        /// for parsing of row values when processing CSV rows into flight records
        /// </summary>
        /// <param name="headers"></param>
        private void ParseHeaders(string[] headers)
        {
            // Get a list of all available flight properties
            IEnumerable<FlightProperty> properties = _factory.Properties.GetProperties();

            // Create an empty field list
            _fields = new List<CsvField>();

            // Iterate over the CSV headers
            for (int i = 0; i < headers.Length; i++)
            {
                // See if this header name matches a flight property. If it does,
                // add a field at this position that maps to that property. If not,
                // just add a simple field at this position
                FlightProperty property = properties.FirstOrDefault(p => p.Name.Equals(headers[i], StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    _fields.Add(new FlightPropertyField
                    {
                        Name = headers[i],
                        Index = i,
                        Property = property
                    });
                }
                else
                {
                    _fields.Add(new CsvField
                    {
                        Name = headers[i],
                        Index = i
                    });
                }
            }
        }

        /// <summary>
        /// Create a new flight object from the specified record
        /// </summary>
        /// <param name="count"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private Flight CreateFlightFromRecord(int count, string[] record)
        {
            Flight flight = new Flight()
            {
                DroneId = GetDroneId(count, record),
                LocationId = GetLocationId(count, record),
                OperatorId = GetOperatorId(count, record),
                Start = GetDateTime(count, record, _settings.StartDateColumnName, _settings.StartTimeColumnName),
                End = GetDateTime(count, record, _settings.EndDateColumnName, _settings.EndTimeColumnName),
                Properties = CreatePropertyValuesFromRecord(count, record)
            };

            return flight;
        }

        /// <summary>
        /// Create flight property values from the specified record
        /// </summary>
        /// <param name="count"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private List<FlightPropertyValue> CreatePropertyValuesFromRecord(int count, string[] record)
        {
            List<FlightPropertyValue> values = new List<FlightPropertyValue>();

            // Iterate over the fields that relate to flight properties
            foreach (FlightPropertyField field in _fields.Where(f => f is FlightPropertyField))
            {
                // Check we have a value for this one
                if (!string.IsNullOrEmpty(record[field.Index]))
                {
                    // Construct a property value object for it
                    try
                    {
                        FlightPropertyValue value = field.GetPropertyValue(record);
                        values.Add(value);
                    }
                    catch (Exception ex)
                    {
                        LastError = new CsvReaderError
                        {
                            Record = count,
                            Message = $"Error getting value for field '{field.Name}' at record {count}",
                            Exception = ex.Message
                        };

                        throw;
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Use the drone name from the specified record to identify the drone Id
        /// </summary>
        /// <param name="count"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private int GetDroneId(int count, string[] record)
        {
            string name = GetMandatoryFieldValue<string>(count, _settings.DroneColumnName, record);
            IEnumerable<Drone> drones = _drones.Where(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (!drones.Any())
            {
                string message = $"Drone '{name}' not found at record {count}";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new DroneNotFoundException(message);
            }
            else if (drones.Count() > 1)
            {
                string message = $"Drone name '{name}' matches more than one drone at record {count}";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new TooManyDronesFoundException(message);
            }

            return drones.First().Id;
        }

        /// <summary>
        /// Use the location name from the specified record to identify the location Id
        /// </summary>
        /// <param name="count"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private int GetLocationId(int count, string[] record)
        {
            string name = GetMandatoryFieldValue<string>(count, _settings.LocationColumnName, record);
            IEnumerable<Location> locations = _locations.Where(l => l.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (!locations.Any())
            {
                string message = $"Location '{name}' not found at record {count}";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new LocationNotFoundException(message);
            }
            else if (locations.Count() > 1)
            {
                string message = $"Location name '{name}' matches more than one location at record {count}";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new TooManyLocationsFoundException(message);
            }

            return locations.First().Id;
        }

        /// <summary>
        /// Use the operator number from the specified record to identify the operator Id
        /// </summary>
        /// <param name="count"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private int GetOperatorId(int count, string[] record)
        {
            string operatorNumber = GetMandatoryFieldValue<string>(count, _settings.OperatorColumnName, record);
            IEnumerable<Operator> operators = _operators.Where(o => o.OperatorNumber.Equals(operatorNumber, StringComparison.OrdinalIgnoreCase));

            if (!operators.Any())
            {
                string message = $"Operator number '{operatorNumber}' not found at record {count}";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new OperatorNotFoundException(message);
            }
            else if (operators.Count() > 1)
            {
                string message = $"Operator number '{operatorNumber}' matches more than one operator at record {count}";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new TooManyOperatorsFoundException(message);
            }

            return operators.First().Id;
        }

        /// <summary>
        /// Return a date and time from the specified record
        /// </summary>
        /// <param name="count"></param>
        /// <param name="record"></param>
        /// <param name="dateColumnName"></param>
        /// <param name="timeColumnName"></param>
        /// <returns></returns>
        private DateTime GetDateTime(int count, string[] record, string dateColumnName, string timeColumnName)
        {
            DateTime date = GetMandatoryFieldValue<DateTime>(count, dateColumnName, record);
            DateTime time = GetMandatoryFieldValue<DateTime>(count, timeColumnName, record);
            return new DateTime(date.Year, date.Month, date.Day,
                                time.Hour, time.Minute, 0);
        }

        /// <summary>
        /// Find and return the value of one of the mandatory (non-flight-property)
        /// fields
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private U GetMandatoryFieldValue<U>(int count, string name, string[] record) where U : IConvertible
        {
            // Locate the field with this name
            CsvField field = _fields.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (field == null)
            {
                // Not found, so an an error to the errors collection and throw
                // an exception
                string message = $"Missing field '{name}' at record '{count}'";
                LastError = new CsvReaderError { Record = count, Message = message };
                throw new FieldNotFoundException(message);
            }

            U value;

            try
            {
                // Attempt to convert the value for this field to type U
                value = field.Get<U>(record);
            }
            catch (Exception ex)
            {
                LastError = new CsvReaderError
                {
                    Record = count,
                    Message = $"Error getting value for field '{name}' at record {count}",
                    Exception = ex.Message
                };

                throw;
            }

            return value;
        }
    }
}
