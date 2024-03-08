using System;
using System.Collections.Generic;
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
    internal class OperatorManager<T> : IOperatorManager where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly IDroneFlightLogFactory<T> _factory;

        internal OperatorManager(IDroneFlightLogFactory<T> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return the operator with the specified Id
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        public Operator GetOperator(int operatorId)
        {
            Operator op = _factory.Context.Operators.Include(o => o.Address).FirstOrDefault(o => o.Id == operatorId);
            ThrowIfOperatorNotFound(op, operatorId);
            return op;
        }

        /// <summary>
        /// Return the operator with the specified Id
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        public async Task<Operator> GetOperatorAsync(int operatorId)
        {
            Operator op = await _factory.Context.Operators.Include(o => o.Address).FirstOrDefaultAsync(o => o.Id == operatorId);
            ThrowIfOperatorNotFound(op, operatorId);
            return op;
        }

        /// <summary>
        /// Get all the current operator details, optionally filtering by address
        /// </summary>
        /// <param name="addressId"></param>
        public IEnumerable<Operator> GetOperators(int? addressId)
        {
            IEnumerable<Operator> operators = (addressId == null) ? _factory.Context.Operators.Include(o => o.Address) :
                                                                    _factory.Context.Operators.Include(o => o.Address)
                                                                                              .Where(o => o.AddressId == addressId);
            return operators;
        }

        /// <summary>
        /// Get all the current operator details, optionally filtering by address
        /// </summary>
        /// <param name="addressId"></param>
        public IAsyncEnumerable<Operator> GetOperatorsAsync(int? addressId)
        {
            IAsyncEnumerable<Operator> operators;

            if (addressId == null)
            {
                operators = _factory.Context.Operators
                                            .Include(o => o.Address)
                                            .AsAsyncEnumerable();
            }
            else
            {
                operators = _factory.Context.Operators
                                            .Include(o => o.Address)
                                            .Where(o => o.AddressId == addressId)
                                            .AsAsyncEnumerable();

            }

            return operators;
        }

        /// <summary>
        /// Add an  operator
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="flyerNumber"></param>
        /// <param name="operatorNumber"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Operator AddOperator(string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId)
        {
            // This will throw an exception if the address doesn't exist
            _factory.Addresses.GetAddress(addressId);

            Operator op = FindOperator(firstnames, surname, addressId);
            ThrowIfOperatorFound(op, firstnames, surname, addressId);

            op = new Operator
            {
                AddressId = addressId,
                DoB = dob,
                FirstNames = firstnames,
                FlyerNumber = flyerNumber,
                OperatorNumber = operatorNumber,
                Surname = surname
            };

            _factory.Context.Operators.Add(op);
            return op;
        }

        /// <summary>
        /// Add an  operator
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="flyerNumber"></param>
        /// <param name="operatorNumber"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<Operator> AddOperatorAsync(string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId)
        {
            // This will throw an exception if the address doesn't exist
            await _factory.Addresses.GetAddressAsync(addressId);

            Operator op = await FindOperatorAsync(firstnames, surname, addressId);
            ThrowIfOperatorFound(op, firstnames, surname, addressId);

            op = new Operator
            {
                AddressId = addressId,
                DoB = dob,
                FirstNames = firstnames,
                FlyerNumber = flyerNumber,
                OperatorNumber = operatorNumber,
                Surname = surname
            };

            await _factory.Context.Operators.AddAsync(op);
            return op;
        }

        /// <summary>
        /// Update an existing operator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="flyerNumber"></param>
        /// <param name="operatorNumber"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Operator UpdateOperator(int id, string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId)
        {
            // This will throw an exception if the address doesn't exist
            _factory.Addresses.GetAddress(addressId);

            Operator existing = FindOperator(firstnames, surname, addressId);
            ThrowIfOperatorFound(existing, firstnames, surname, addressId);

            Operator op = GetOperator(id);
            op.FirstNames = firstnames.CleanString();
            op.Surname = surname.CleanString();
            op.DoB = dob;
            op.FlyerNumber = flyerNumber.CleanString();
            op.OperatorNumber = operatorNumber.CleanString();
            op.AddressId = addressId;
            return op;
        }

        /// <summary>
        /// Update an existing operator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="dob"></param>
        /// <param name="flyerNumber"></param>
        /// <param name="operatorNumber"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<Operator> UpdateOperatorAsync(int id, string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId)
        {
            // This will throw an exception if the address doesn't exist
            await _factory.Addresses.GetAddressAsync(addressId);

            Operator existing = await FindOperatorAsync(firstnames, surname, addressId);
            ThrowIfOperatorFound(existing, firstnames, surname, addressId);

            Operator op = await GetOperatorAsync(id);
            op.FirstNames = firstnames.CleanString();
            op.Surname = surname.CleanString();
            op.DoB = dob;
            op.FlyerNumber = flyerNumber.CleanString();
            op.OperatorNumber = operatorNumber.CleanString();
            op.AddressId = addressId;
            return op;
        }

        /// <summary>
        /// Update the address for the specified operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="addressId"></param>
        public void SetOperatorAddress(int operatorId, int addressId)
        {
            // This will throw an exception if the address doesn't exist
            _factory.Addresses.GetAddress(addressId);

            Operator op = _factory.Context.Operators.FirstOrDefault(o => o.Id == operatorId);
            ThrowIfOperatorNotFound(op, operatorId);
            op.AddressId = addressId;
        }

        /// <summary>
        /// Update the address for the specified operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="addressId"></param>
        public async Task SetOperatorAddressAsync(int operatorId, int addressId)
        {
            // This will throw an exception if the address doesn't exist
            await _factory.Addresses.GetAddressAsync(addressId);

            Operator op = await _factory.Context.Operators.FirstOrDefaultAsync(o => o.Id == operatorId);
            ThrowIfOperatorNotFound(op, operatorId);
            op.AddressId = addressId;
        }

        /// <summary>
        /// Find an operator based on their name and address details
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Operator FindOperator(string firstnames, string surname, int addressId)
        {
            firstnames = firstnames.CleanString();
            surname = surname.CleanString();

            return _factory.Context.Operators
                                   .Include(o => o.Address)
                                   .FirstOrDefault(a => (a.FirstNames == firstnames) &&
                                                        (a.Surname == surname) &&
                                                        (a.AddressId == addressId));
        }

        /// <summary>
        /// Find an operator based on their name and address details
        /// </summary>
        /// <param name="firstnames"></param>
        /// <param name="surname"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<Operator> FindOperatorAsync(string firstnames, string surname, int addressId)
        {
            firstnames = firstnames.CleanString();
            surname = surname.CleanString();

            return await _factory.Context.Operators
                                         .Include(o => o.Address)
                                         .FirstOrDefaultAsync(a => (a.FirstNames == firstnames) &&
                                                                   (a.Surname == surname) &&
                                                                   (a.AddressId == addressId));
        }

        /// <summary>
        /// Throw an exception if an operator is not found
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operatorId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfOperatorNotFound(Operator op, int operatorId)
        {
            if (op == null)
            {
                string message = $"Operator with ID {operatorId} not found";
                throw new OperatorNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if an operator already exists
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operatorId"></param>
        [ExcludeFromCodeCoverage]
        private void ThrowIfOperatorFound(Operator op, string firstnames, string surname, int addressId)
        {
            if (op != null)
            {
                string message = $"Operator {firstnames} {surname} already exists at address {addressId}";
                throw new OperatorExistsException(message);
            }
        }
    }
}
