using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Extensions;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneFlightLog.Data.Logic
{
    internal class MaintainerManager<T> : IMaintainerManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogDbContext _context;

        internal MaintainerManager(IDroneFlightLogDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the maintainer with the specified Id
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <returns></returns>
        public Maintainer GetMaintainer(int maintainerId)
        {
            Maintainer maintainer = _context.Maintainers.FirstOrDefault(m => m.Id == maintainerId);
            ThrowIfMaintainerNotFound(maintainer, maintainerId);
            return maintainer;
        }

        /// <summary>
        /// Return the maintainer with the specified Id
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <returns></returns>
        public async Task<Maintainer> GetMaintainerAsync(int maintainerId)
        {
            Maintainer maintainer = await _context.Maintainers.FirstOrDefaultAsync(m => m.Id == maintainerId);
            ThrowIfMaintainerNotFound(maintainer, maintainerId);
            return maintainer;
        }

        /// <summary>
        /// Get all the current maintainer details
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Maintainer> GetMaintainers()
        {
            IEnumerable<Maintainer> maintainers = _context.Maintainers;
            return maintainers;
        }

        /// <summary>
        /// Get all the current maintainer details
        /// </summary>
        public IAsyncEnumerable<Maintainer> GetMaintainersAsync()
        {
            IAsyncEnumerable<Maintainer> maintainers = _context.Maintainers.AsAsyncEnumerable();
            return maintainers;
        }

        /// <summary>
        /// Add a maintainer
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public Maintainer AddMaintainer(string firstnames, string surname)
        {
            Maintainer maintainer = FindMaintainer(firstnames, surname);
            ThrowIfMaintainerFound(maintainer, firstnames, surname);

            maintainer = new Maintainer
            {
                FirstNames = firstnames.CleanString(),
                Surname = surname.CleanString()
            };

            _context.Maintainers.Add(maintainer);
            return maintainer;
        }

        /// <summary>
        /// Add a maintainer
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public async Task<Maintainer> AddMaintainerAsync(string firstnames, string surname)
        {
            Maintainer maintainer = await FindMaintainerAsync(firstnames, surname);
            ThrowIfMaintainerFound(maintainer, firstnames, surname);

            maintainer = new Maintainer
            {
                FirstNames = firstnames.CleanString(),
                Surname = surname.CleanString()
            };

            await _context.Maintainers.AddAsync(maintainer);
            return maintainer;
        }

        /// <summary>
        /// Update a maintainers details
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public Maintainer UpdateMaintainer(int maintainerId, string firstnames, string surname)
        {
            // Check there isn't already a maintainer with those details
            Maintainer existing = FindMaintainer(firstnames, surname);
            ThrowIfMaintainerFound(existing, firstnames, surname);

            // Get the current maintainer and update their details
            Maintainer maintainer = GetMaintainer(maintainerId);
            maintainer.FirstNames = firstnames.CleanString();
            maintainer.Surname = surname.CleanString();
            return maintainer;
        }

        /// <summary>
        /// Update a maintainers details
        /// </summary>
        /// <param name="maintainerId"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public async Task<Maintainer> UpdateMaintainerAsync(int maintainerId, string firstnames, string surname)
        {
            // Check there isn't already a maintainer with those details
            Maintainer existing = await FindMaintainerAsync(firstnames, surname);
            ThrowIfMaintainerFound(existing, firstnames, surname);

            // Get the current maintainer and update their details
            Maintainer maintainer = await GetMaintainerAsync(maintainerId);
            maintainer.FirstNames = firstnames.CleanString();
            maintainer.Surname = surname.CleanString();
            return maintainer;
        }

        /// <summary>
        /// Find a maintainer based on their name
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public Maintainer FindMaintainer(string firstnames, string surname)
        {
            firstnames = firstnames.CleanString();
            surname = surname.CleanString();

            return _context.Maintainers
                                   .FirstOrDefault(a => (a.FirstNames == firstnames) &&
                                                        (a.Surname == surname));
        }

        /// <summary>
        /// Find a maintainer based on their name
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public async Task<Maintainer> FindMaintainerAsync(string firstnames, string surname)
        {
            firstnames = firstnames.CleanString();
            surname = surname.CleanString();

            return await _context.Maintainers
                                 .FirstOrDefaultAsync(a => (a.FirstNames == firstnames) &&
                                                           (a.Surname == surname));
        }

        /// <summary>
        /// Throw an exception if a maintainer is not found
        /// </summary>
        /// <param name="maintainer"></param>
        /// <param name="maintainerId"></param>
        [ExcludeFromCodeCoverage]
        private static void ThrowIfMaintainerNotFound(Maintainer maintainer, int maintainerId)
        {
            if (maintainer == null)
            {
                string message = $"Maintainer with ID {maintainerId} not found";
                throw new MaintainerNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a maintainer already exists
        /// </summary>
        /// <param name="maintainer"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        [ExcludeFromCodeCoverage]
        private static void ThrowIfMaintainerFound(Maintainer maintainer, string firstnames, string surname)
        {
            if (maintainer != null)
            {
                string message = $"Maintainer {firstnames} {surname} already exists";
                throw new MaintainerExistsException(message);
            }
        }
    }
}
