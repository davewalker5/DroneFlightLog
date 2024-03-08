using System;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Importer.Entities
{
    public class CsvField
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public T Get<T>(string[] fields) where T : IConvertible
        {
            return (T)Convert.ChangeType(fields[Index], typeof(T));
        }
    }
}
