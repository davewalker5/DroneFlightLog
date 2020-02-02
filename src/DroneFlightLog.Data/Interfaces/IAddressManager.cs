using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IAddressManager
    {
        Address AddAddress(string number, string street, string town, string county, string postcode, string country);
        Address FindAddress(string number, string postcode, string country);
        Address GetAddress(int id);
    }
}