using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IModelManager
    {
        Model AddModel(string name, int manufacturerId);
        Task<Model> AddModelAsync(string name, int manufacturerId);
        Model GetModel(int modelId);
        Task<Model> GetModelAsync(int modelId);
        IEnumerable<Model> GetModels(int? manufacturerId);
        IAsyncEnumerable<Model> GetModelsAsync(int? manufacturerId);
    }
}