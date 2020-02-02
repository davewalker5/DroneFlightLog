using System.Collections.Generic;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IModelManager
    {
        Model AddModel(string name, int manufacturerId);
        Model GetModel(int modelId);
        IEnumerable<Model> GetModels(int? manufacturerId);
    }
}