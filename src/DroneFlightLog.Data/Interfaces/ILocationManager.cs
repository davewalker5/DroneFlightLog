using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface ILocationManager
    {
        Location AddLocation(string name);
        Task<Location> AddLocationAsync(string name);
        Location FindLocation(string name);
        Task<Location> FindLocationAsync(string name);
        Location GetLocation(int locationId);
        Task<Location> GetLocationAsync(int locationId);
        IEnumerable<Location> GetLocations();
        IAsyncEnumerable<Location> GetLocationsAsync();
    }
}