using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PropertyValueNotFoundException : Exception
    {
        public PropertyValueNotFoundException()
        {
        }

        public PropertyValueNotFoundException(string message) : base(message)
        {
        }

        public PropertyValueNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
