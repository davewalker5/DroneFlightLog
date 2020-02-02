using System;
using System.Linq;
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

            if (address == null)
            {
                string message = $"Address with ID {addressId} not found";
                throw new AddressNotFoundException(message);
            }

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

            return _context.Addresses.FirstOrDefault(a => a.Number.Equals(number, StringComparison.OrdinalIgnoreCase) &&
                                                          a.Postcode.Equals(postcode, StringComparison.OrdinalIgnoreCase) &&
                                                          a.Country.Equals(country, StringComparison.OrdinalIgnoreCase));
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
            if (FindAddress(number, postcode, country) != null)
            {
                throw new AddressExistsException("Address already exists");
            }

            Address address = new Address
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
    }
}
