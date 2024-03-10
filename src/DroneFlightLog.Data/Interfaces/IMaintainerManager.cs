using DroneFlightLog.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IMaintainerManager
    {
        Maintainer AddMaintainer(string firstnames, string surname);
        Task<Maintainer> AddMaintainerAsync(string firstnames, string surname);
        Maintainer FindMaintainer(string firstnames, string surname);
        Task<Maintainer> FindMaintainerAsync(string firstnames, string surname);
        Maintainer GetMaintainer(int maintainerId);
        Task<Maintainer> GetMaintainerAsync(int maintainerId);
        IEnumerable<Maintainer> GetMaintainers();
        IAsyncEnumerable<Maintainer> GetMaintainersAsync();
        Maintainer UpdateMaintainer(int maintainerId, string firstnames, string surname);
        Task<Maintainer> UpdateMaintainerAsync(int maintainerId, string firstnames, string surname);
    }
}