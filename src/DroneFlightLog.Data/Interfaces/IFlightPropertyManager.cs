using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IFlightPropertyManager
    {
        FlightProperty AddProperty(string name, FlightPropertyDataType type, bool isSingleInstance);
        Task<FlightProperty> AddPropertyAsync(string name, FlightPropertyDataType type, bool isSingleInstance);
        FlightPropertyValue AddPropertyValue(int flightId, int propertyId, object value);
        Task<FlightPropertyValue> AddPropertyValueAsync(int flightId, int propertyId, object value);
        IEnumerable<FlightProperty> GetProperties();
        IAsyncEnumerable<FlightProperty> GetPropertiesAsync();
        IEnumerable<FlightPropertyValue> GetPropertyValues(int flightId);
        IAsyncEnumerable<FlightPropertyValue> GetPropertyValuesAsync(int flightId);
        FlightProperty UpdateProperty(int id, string name);
        Task<FlightProperty> UpdatePropertyAsync(int id, string name);
        FlightPropertyValue UpdatePropertyValue(int id, object value);
        Task<FlightPropertyValue> UpdatePropertyValueAsync(int id, object value);
    }
}