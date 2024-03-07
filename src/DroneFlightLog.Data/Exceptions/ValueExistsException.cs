using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ValueExistsException : Exception
    {
        public ValueExistsException()
        {
        }

        public ValueExistsException(string message) : base(message)
        {
        }

        public ValueExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
