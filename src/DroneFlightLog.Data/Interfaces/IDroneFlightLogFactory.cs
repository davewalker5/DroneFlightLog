using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Interfaces
{
    public interface IDroneFlightLogFactory<T> where T : DbContext, IDroneFlightLogDbContext
    {
        T Context { get; }
        IFlightPropertyManager Properties { get; }
        IAddressManager Addresses { get; }
        ILocationManager Locations { get; }
        IManufacturerManager Manufacturers { get; }
        IModelManager Models { get; }
        IDroneManager Drones { get; }
        IOperatorManager Operators { get; }
        IFlightManager Flights { get; }
        IUserManager Users { get; }
    }
}