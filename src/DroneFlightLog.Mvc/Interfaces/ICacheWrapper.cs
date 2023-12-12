using System.Collections.Generic;

namespace DroneFlightLog.Mvc.Interfaces
{
    public interface ICacheWrapper
    {
        void Dispose();
        T Get<T>(string key);
        IEnumerable<string> GetKeys();
        void Remove(string key);
        T Set<T>(string key, T item, int duration);
    }
}