using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Importer.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException()
        {
        }

        public FieldNotFoundException(string message) : base(message)
        {
        }

        public FieldNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
