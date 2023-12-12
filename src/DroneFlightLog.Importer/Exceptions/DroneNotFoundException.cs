using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Importer.Exceptions
{
    [Serializable]
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
