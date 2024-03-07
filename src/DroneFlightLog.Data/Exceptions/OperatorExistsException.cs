using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class OperatorExistsException : Exception
    {
        public OperatorExistsException()
        {
        }

        public OperatorExistsException(string message) : base(message)
        {
        }

        public OperatorExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
