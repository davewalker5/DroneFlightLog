using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ModelExistsException : Exception
    {
        public ModelExistsException()
        {
        }

        public ModelExistsException(string message) : base(message)
        {
        }

        public ModelExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}