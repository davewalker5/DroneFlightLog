using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface ILocationManager
    {
        Location AddLocation(string name);
        Location FindLocation(string name);
        Location GetLocation(int locationId);
        IEnumerable<Location> GetLocations();
    }
}