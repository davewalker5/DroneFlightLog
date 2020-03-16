using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IOperatorManager
    {
        Operator AddOperator(string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId);
        Task<Operator> AddOperatorAsync(string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId);
        Operator FindOperator(string firstnames, string surname, int addressId);
        Task<Operator> FindOperatorAsync(string firstnames, string surname, int addressId);
        Operator GetOperator(int operatorId);
        Task<Operator> GetOperatorAsync(int operatorId);
        IEnumerable<Operator> GetOperators(int? addressId);
        IAsyncEnumerable<Operator> GetOperatorsAsync(int? addressId);
        void SetOperatorAddress(int operatorId, int addressId);
        Task SetOperatorAddressAsync(int operatorId, int addressId);
        Operator UpdateOperator(int id, string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId);
        Task<Operator> UpdateOperatorAsync(int id, string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId);
    }
}