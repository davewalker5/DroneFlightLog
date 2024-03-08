using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PropertyExistsException : Exception
    {
        public PropertyExistsException()
        {
        }

        public PropertyExistsException(string message) : base(message)
        {
        }

        public PropertyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
