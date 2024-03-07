using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IUserManager
    {
        FlightLogUser AddUser(string userName, string password);
        Task<FlightLogUser> AddUserAsync(string userName, string password);
        bool Authenticate(string userName, string password);
        Task<bool> AuthenticateAsync(string userName, string password);
        void DeleteUser(string userName);
        Task DeleteUserAsync(string userName);
        FlightLogUser GetUser(int userId);
        FlightLogUser GetUser(string userName);
        Task<FlightLogUser> GetUserAsync(int userId);
        Task<FlightLogUser> GetUserAsync(string userName);
        IEnumerable<FlightLogUser> GetUsers();
        IAsyncEnumerable<FlightLogUser> GetUsersAsync();
        void SetPassword(string userName, string password);
        Task SetPasswordAsync(string userName, string password);
    }
}