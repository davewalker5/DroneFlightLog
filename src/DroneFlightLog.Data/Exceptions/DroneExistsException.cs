using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DroneExistsException : Exception
    {
        public DroneExistsException()
        {
        }

        public DroneExistsException(string message) : base(message)
        {
        }

        public DroneExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
