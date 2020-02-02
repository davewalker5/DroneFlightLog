using System;
using System.Collections.Generic;
using System.Linq;
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

            if (op == null)
            {
                string message = $"Operator with ID {operatorId} not found";
                throw new OperatorNotFoundException(message);
            }

            return op;
        }

        /// <summary>
        /// Get all the current operator details, optionally filtering by address
        /// </summary>
        /// <param name="addressId"></param>
        public IEnumerable<Operator> GetOperators(int? addressId)
        {
            IEnumerable<Operator> operators = (addressId == null) ? _factory.Context.Operators.Include(o => o.Address) : _factory.Context.Operators.Include(o => o.Address).Where(o => o.AddressId == addressId);
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
            if (FindOperator(firstnames, surname, addressId) != null)
            {
                string message = $"Operator {firstnames} {surname} already exists at address {addressId}";
                throw new OperatorExistsException(message);
            }

            Operator op = new Operator
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
        /// Update the address for the specified operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="addressId"></param>
        public void SetOperatorAddress(int operatorId, int addressId)
        {
            // This will throw an exception if the adress doesn't exist
            _factory.Addresses.GetAddress(addressId);

            Operator op = _factory.Context.Operators.FirstOrDefault(o => o.Id == operatorId);
            if (op == null)
            {
                string message = $"Operator with ID {operatorId} not found";
                throw new OperatorNotFoundException(message);
            }

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

            return _factory.Context.Operators.Include(o => o.Address).FirstOrDefault(a => a.FirstNames.Equals(firstnames, StringComparison.OrdinalIgnoreCase) &&
                                                                                            a.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase) &&
                                                                                            (a.AddressId == addressId));
        }
    }
}
