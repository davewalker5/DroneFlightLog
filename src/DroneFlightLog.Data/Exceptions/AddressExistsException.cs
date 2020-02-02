using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AddressExistsException : Exception
    {
        public AddressExistsException()
        {
        }

        public AddressExistsException(string message) : base(message)
        {
        }

        public AddressExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
