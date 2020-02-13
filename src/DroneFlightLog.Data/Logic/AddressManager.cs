using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Extensions;
using DroneFlightLog.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Logic
{
    internal class AddressManager<T> : IAddressManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly T _context;

        public AddressManager(T context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the address with the specified Id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Address GetAddress(int addressId)
        {
            Address address = _context.Addresses.FirstOrDefault(a => a.Id == addressId);
            ThrowIfAddressNotFound(address, addressId);
            return address;
        }

        /// <summary>
        /// Return the address with the specified Id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<Address> GetAddressAsync(int addressId)
        {
            Address address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);
            ThrowIfAddressNotFound(address, addressId);
            return address;
        }

        /// <summary>
        /// Find an existing address
        /// </summary>
        /// <param name="number"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public Address FindAddress(string number, string postcode, string country)
        {
            number = number.CleanString();
            postcode = postcode.CleanString();
            country = country.CleanString();

            return _context.Addresses.FirstOrDefault(a => (a.Number == number) &&
                                                          (a.Postcode == postcode) &&
                                                          (a.Country == country));
        }

        /// <summary>
        /// Find an existing address
        /// </summary>
        /// <param name="number"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Address> FindAddressAsync(string number, string postcode, string country)
        {
            number = number.CleanString();
            postcode = postcode.CleanString();
            country = country.CleanString();

            return await _context.Addresses.FirstOrDefaultAsync(a => (a.Number == number) &&
                                                                     (a.Postcode == postcode) &&
                                                                     (a.Country == country));
        }

        /// <summary>
        /// Add a new address to the database
        /// </summary>
        /// <param name="number"></param>
        /// <param name="street"></param>
        /// <param name="town"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        public Address AddAddress(string number, string street, string town, string county, string postcode, string country)
        {
            Address address = FindAddress(number, postcode, country);
            ThrowIfAddressFound(address);

            address = new Address
            {
                Number = number.CleanString(),
                Street = street.CleanString(),
                Town = town.CleanString(),
                County = county.CleanString(),
                Postcode = postcode.CleanString(),
                Country = country.CleanString()
            };

            _context.Addresses.Add(address);
            return address;
        }

        /// <summary>
        /// Add a new address to the database
        /// </summary>
        /// <param name="number"></param>
        /// <param name="street"></param>
        /// <param name="town"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <param name="country"></param>
        public async Task<Address> AddAddressAsync(string number, string street, string town, string county, string postcode, string country)
        {
            Address address = await FindAddressAsync(number, postcode, country);
            ThrowIfAddressFound(address);

            address = new Address
            {
                Number = number.CleanString(),
                Street = street.CleanString(),
                Town = town.CleanString(),
                County = county.CleanString(),
                Postcode = postcode.CleanString(),
                Country = country.CleanString()
            };

            await _context.Addresses.AddAsync(address);
            return address;
        }

        /// <summary>
        /// Throw an exception if an address doesn't exist
        /// </summary>
        /// <param name="address"></param>
        /// <param name="addressId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfAddressNotFound(Address address, int addressId)
        {
            if (address == null)
            {
                string message = $"Address with ID {addressId} not found";
                throw new AddressNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if an address already exists
        /// </summary>
        /// <param name="address"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfAddressFound(Address address)
        {
            if (address != null)
            {
                throw new AddressExistsException("Address already exists");
            }
        }
    }
}
