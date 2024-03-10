using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class MaintainerNotFoundException : Exception
    {
        public MaintainerNotFoundException()
        {
        }

        public MaintainerNotFoundException(string message) : base(message)
        {
        }

        public MaintainerNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
