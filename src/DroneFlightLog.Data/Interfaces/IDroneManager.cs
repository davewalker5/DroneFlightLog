using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IDroneManager
    {
        Drone AddDrone(string name, string serialNumber, int modelId);
        Drone FindDrone(string serialNumber, int modelId);
        Drone GetDrone(int droneId);
        IEnumerable<Drone> GetDrones(int? modelId);
    }
}