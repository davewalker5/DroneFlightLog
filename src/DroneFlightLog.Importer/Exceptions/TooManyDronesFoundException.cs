using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Importer.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class TooManyDronesFoundException : Exception
    {
        public TooManyDronesFoundException()
        {
        }

        public TooManyDronesFoundException(string message) : base(message)
        {
        }

        public TooManyDronesFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
