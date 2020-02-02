﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class LocationExistsException : Exception
    {
        public LocationExistsException()
        {
        }

        public LocationExistsException(string message) : base(message)
        {
        }

        public LocationExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
