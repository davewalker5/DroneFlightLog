using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IManufacturerManager
    {
        Manufacturer AddManufacturer(string name);
        Task<Manufacturer> AddManufacturerAsync(string name);
        Manufacturer FindManufacturer(string name);
        Task<Manufacturer> FindManufacturerAsync(string name);
        Manufacturer GetManufacturer(int manufacturerId);
        Task<Manufacturer> GetManufacturerAsync(int manufacturerId);
        IEnumerable<Manufacturer> GetManufacturers();
        IAsyncEnumerable<Manufacturer> GetManufacturersAsync();
        Manufacturer UpdateManufacturer(int id, string name);
        Task<Manufacturer> UpdateManufacturerAsync(int id, string name);
    }
}