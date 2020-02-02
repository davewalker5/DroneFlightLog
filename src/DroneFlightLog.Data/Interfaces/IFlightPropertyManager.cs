using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IFlightPropertyManager
    {
        FlightProperty AddProperty(string name, FlightPropertyDataType type, bool isSingleInstance);
        FlightPropertyValue AddPropertyValue(int flightId, int propertyId, object value);
        IEnumerable<FlightProperty> GetProperties();
        IEnumerable<FlightPropertyValue> GetPropertyValues(int flightId);
    }
}