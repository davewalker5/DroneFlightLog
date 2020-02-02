using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DroneNotFoundException : Exception
    {
        public DroneNotFoundException()
        {
        }

        public DroneNotFoundException(string message) : base(message)
        {
        }

        public DroneNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
