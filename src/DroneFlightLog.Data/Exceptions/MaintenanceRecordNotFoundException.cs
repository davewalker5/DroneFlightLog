using System;
using System.Diagnostics.CodeAnalysis;

namespace DroneFlightLog.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class MaintenanceRecordNotFoundException : Exception
    {
        public MaintenanceRecordNotFoundException()
        {
        }

        public MaintenanceRecordNotFoundException(string message) : base(message)
        {
        }

        public MaintenanceRecordNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}