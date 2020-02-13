using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IAddressManager
    {
        Address AddAddress(string number, string street, string town, string county, string postcode, string country);
        Task<Address> AddAddressAsync(string number, string street, string town, string county, string postcode, string country);
        Address FindAddress(string number, string postcode, string country);
        Task<Address> FindAddressAsync(string number, string postcode, string country);
        Address GetAddress(int id);
        Task<Address> GetAddressAsync(int addressId);
    }
}