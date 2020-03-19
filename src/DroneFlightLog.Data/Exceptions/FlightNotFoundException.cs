using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class FlightNotFoundException : Exception
    {
        public FlightNotFoundException()
        {
        }

        public FlightNotFoundException(string message) : base(message)
        {
        }

        public FlightNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
