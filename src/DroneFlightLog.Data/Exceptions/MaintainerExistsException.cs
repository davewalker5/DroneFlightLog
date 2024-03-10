using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class MaintainerExistsException : Exception
    {
        public MaintainerExistsException()
        {
        }

        public MaintainerExistsException(string message) : base(message)
        {
        }

        public MaintainerExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}