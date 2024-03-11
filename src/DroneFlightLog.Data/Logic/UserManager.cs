using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DroneFlightLog.Data.Logic
{
    public class UserManager<T> : IUserManager where T : DbContext, IDroneFlightLogDbContext
    {
        private Lazy<PasswordHasher<string>> _hasher;
        private readonly T _context;

        public UserManager(T context)
        {
            _hasher = new Lazy<PasswordHasher<string>>(() => new PasswordHasher<string>());
            _context = context;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FlightLogUser GetUser(int userId)
        {
            FlightLogUser user = _context.FlightLogUsers.FirstOrDefault(u => u.Id == userId);
            ThrowIfUserNotFound(user, userId);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<FlightLogUser> GetUserAsync(int userId)
        {
            FlightLogUser user = await _context.FlightLogUsers.FirstOrDefaultAsync(u => u.Id == userId);
            ThrowIfUserNotFound(user, userId);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FlightLogUser GetUser(string userName)
        {
            FlightLogUser user = _context.FlightLogUsers.FirstOrDefault(u => u.UserName == userName);
            ThrowIfUserNotFound(user, userName);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<FlightLogUser> GetUserAsync(string userName)
        {
            FlightLogUser user = await _context.FlightLogUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            ThrowIfUserNotFound(user, userName);
            return user;
        }

        /// <summary>
        /// Get all the current user details
        /// </summary>
        public IEnumerable<FlightLogUser> GetUsers() =>
            _context.FlightLogUsers;

        /// <summary>
        /// Get all the current user details
        /// </summary>
        public IAsyncEnumerable<FlightLogUser> GetUsersAsync() =>
            _context.FlightLogUsers.AsAsyncEnumerable();

        /// <summary>
        /// Add a new user, given their details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public FlightLogUser AddUser(string userName, string password)
        {
            FlightLogUser user = _context.FlightLogUsers.FirstOrDefault(u => u.UserName == userName);
            ThrowIfUserFound(user, userName);

            user = new FlightLogUser
            {
                UserName = userName,
                Password = _hasher.Value.HashPassword(userName, password)
            };

            _context.FlightLogUsers.Add(user);
            _context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Add a new user, given their details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<FlightLogUser> AddUserAsync(string userName, string password)
        {
            FlightLogUser user = await _context.FlightLogUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            ThrowIfUserFound(user, userName);

            user = new FlightLogUser
            {
                UserName = userName,
                Password = _hasher.Value.HashPassword(userName, password)
            };

            await _context.FlightLogUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string userName, string password)
        {
            FlightLogUser user = GetUser(userName);
            PasswordVerificationResult result = _hasher.Value.VerifyHashedPassword(userName, user.Password, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _hasher.Value.HashPassword(userName, password);
                _context.SaveChanges();
            }
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            FlightLogUser user = await GetUserAsync(userName);
            PasswordVerificationResult result = _hasher.Value.VerifyHashedPassword(userName, user.Password, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _hasher.Value.HashPassword(userName, password);
                await _context.SaveChangesAsync();
            }
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void SetPassword(string userName, string password)
        {
            FlightLogUser user = GetUser(userName);
            user.Password = _hasher.Value.HashPassword(userName, password);
            _context.SaveChanges();
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task SetPasswordAsync(string userName, string password)
        {
            FlightLogUser user = await GetUserAsync(userName);
            user.Password = _hasher.Value.HashPassword(userName, password);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="userName"></param>
        public void DeleteUser(string userName)
        {
            FlightLogUser user = GetUser(userName);
            _context.FlightLogUsers.Remove(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="userName"></param>
        public async Task DeleteUserAsync(string userName)
        {
            FlightLogUser user = await GetUserAsync(userName);
            _context.FlightLogUsers.Remove(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Throw an exception if a user doesn't exist
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfUserNotFound(FlightLogUser user, object userId)
        {
            if (user == null)
            {
                string message = $"User {userId} not found";
                throw new UserNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a user already exists
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfUserFound(FlightLogUser user, object userId)
        {
            if (user != null)
            {
                throw new UserExistsException($"User {userId} already exists");
            }
        }
    }
}
