using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ManufacturerExistsException : Exception
    {
        public ManufacturerExistsException()
        {
        }

        public ManufacturerExistsException(string message) : base(message)
        {
        }

        public ManufacturerExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
