using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Importer.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class TooManyLocationsFoundException : Exception
    {
        public TooManyLocationsFoundException()
        {
        }

        public TooManyLocationsFoundException(string message) : base(message)
        {
        }

        public TooManyLocationsFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
