using System;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Data.Logic;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Factory
{
    public class DroneFlightLogFactory<T> : IDroneFlightLogFactory<T> where T : DbContext, IDroneFlightLogDbContext
    {
        private readonly Lazy<IFlightPropertyManager> _properties;
        private readonly Lazy<IAddressManager> _addresses;
        private readonly Lazy<IOperatorManager> _operators;
        private readonly Lazy<IManufacturerManager> _manufacturers;
        private readonly Lazy<IModelManager> _models;
        private readonly Lazy<IDroneManager> _drones;
        private readonly Lazy<ILocationManager> _locations;
        private readonly Lazy<IFlightManager> _flights;
        private readonly Lazy<IUserManager> _users;
        private readonly Lazy<IMaintainerManager> _maintainers;
        private readonly Lazy<IMaintenanceRecordManager> _maintenanceRecords;

        public DroneFlightLogFactory(T context)
        {
            Context = context;
            _properties = new Lazy<IFlightPropertyManager>(() => new FlightPropertyManager<T>(context));
            _addresses = new Lazy<IAddressManager>(() => new AddressManager<T>(context));
            _operators = new Lazy<IOperatorManager>(() => new OperatorManager<T>(this));
            _manufacturers = new Lazy<IManufacturerManager>(() => new ManufacturerManager<T>(context));
            _models = new Lazy<IModelManager>(() => new ModelManager<T>(this));
            _drones = new Lazy<IDroneManager>(() => new DroneManager<T>(this));
            _locations = new Lazy<ILocationManager>(() => new LocationManager<T>(context));
            _flights = new Lazy<IFlightManager>(() => new FlightManager<T>(this));
            _users = new Lazy<IUserManager>(() => new UserManager<T>(context));
            _maintainers = new Lazy<IMaintainerManager>(() => new MaintainerManager<T>(context));
            _maintenanceRecords = new Lazy<IMaintenanceRecordManager>(() => new MaintenanceRecordManager<T>(this));
        }

        public T Context { get; private set; }
        public IFlightPropertyManager Properties { get { return _properties.Value; } }
        public IAddressManager Addresses { get { return _addresses.Value; } }
        public IOperatorManager Operators { get { return _operators.Value; } }
        public IManufacturerManager Manufacturers { get { return _manufacturers.Value; } }
        public IModelManager Models { get { return _models.Value; } }
        public IDroneManager Drones { get { return _drones.Value; } }
        public ILocationManager Locations { get { return _locations.Value; } }
        public IFlightManager Flights { get { return _flights.Value; } }
        public IUserManager Users { get { return _users.Value; } }
        public IMaintainerManager Maintainers { get { return _maintainers.Value; } }
        public IMaintenanceRecordManager MaintenanceRecords { get { return _maintenanceRecords.Value; } }
    }
}
