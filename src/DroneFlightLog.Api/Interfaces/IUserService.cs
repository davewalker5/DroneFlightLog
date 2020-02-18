using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Api.Interfaces
{
    public interface IUserService
    {
        Task<FlightLogUser> AddUserAsync(string userName, string password);
        Task<string> AuthenticateAsync(string userName, string password);
        Task SetUserPasswordAsync(string userName, string password);
    }
}
