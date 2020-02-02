using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IManufacturerManager
    {
        Manufacturer AddManufacturer(string name);
        Manufacturer FindManufacturer(string name);
        Manufacturer GetManufacturer(int manufacturerId);
        IEnumerable<Manufacturer> GetManufacturers();
    }
}