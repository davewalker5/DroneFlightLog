using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ModelNotFoundException : Exception
    {
        public ModelNotFoundException()
        {
        }

        public ModelNotFoundException(string message) : base(message)
        {
        }

        public ModelNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
