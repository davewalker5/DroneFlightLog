using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Importer.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class TooManyOperatorsFoundException : Exception
    {
        public TooManyOperatorsFoundException()
        {
        }

        public TooManyOperatorsFoundException(string message) : base(message)
        {
        }

        public TooManyOperatorsFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
