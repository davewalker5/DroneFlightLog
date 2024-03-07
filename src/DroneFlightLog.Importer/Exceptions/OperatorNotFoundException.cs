using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Importer.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OperatorNotFoundException : Exception
    {
        public OperatorNotFoundException()
        {
        }

        public OperatorNotFoundException(string message) : base(message)
        {
        }

        public OperatorNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
