using System;
using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IOperatorManager
    {
        Operator AddOperator(string firstnames, string surname, DateTime dob, string flyerNumber, string operatorNumber, int addressId);
        Operator FindOperator(string firstnames, string surname, int addressId);
        Operator GetOperator(int operatorId);
        IEnumerable<Operator> GetOperators(int? addressId);
        void SetOperatorAddress(int operatorId, int addressId);
    }
}