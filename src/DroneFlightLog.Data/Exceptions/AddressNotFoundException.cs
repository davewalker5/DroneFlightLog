using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AddressNotFoundException : Exception
    {
        public AddressNotFoundException()
        {
        }

        public AddressNotFoundException(string message) : base(message)
        {
        }

        public AddressNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
