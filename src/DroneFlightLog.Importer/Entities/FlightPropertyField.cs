using System;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Importer.Entities
{
    public class FlightPropertyField : CsvField
    {
        public FlightProperty Property { get; set; }

        public FlightPropertyValue GetPropertyValue(string[] fields)
        {
            FlightPropertyValue value = new FlightPropertyValue
            {
                Property = Property,
                PropertyId = Property.Id
            };

            switch (Property.DataType)
            {
                case FlightPropertyDataType.Date:
                    value.DateValue = Get<DateTime>(fields);
                    break;
                case FlightPropertyDataType.Number:
                    value.NumberValue = Get<decimal>(fields);
                    break;
                case FlightPropertyDataType.String:
                    value.StringValue = fields[Index];
                    break;
                default:
                    break;
            }

            return value;
        }
    }
}
