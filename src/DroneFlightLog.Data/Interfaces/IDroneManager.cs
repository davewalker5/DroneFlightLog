using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IDroneManager
    {
        Drone AddDrone(string name, string serialNumber, int modelId);
        Task<Drone> AddDroneAsync(string name, string serialNumber, int modelId);
        Drone FindDrone(string serialNumber, int modelId);
        Task<Drone> FindDroneAsync(string serialNumber, int modelId);
        Drone GetDrone(int droneId);
        Task<Drone> GetDroneAsync(int droneId);
        IEnumerable<Drone> GetDrones(int? modelId);
        IAsyncEnumerable<Drone> GetDronesAsync(int? modelId);
    }
}