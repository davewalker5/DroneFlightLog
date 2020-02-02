using System;
using DroneFlightLog.Data.Interfaces;
using DroneFlightLog.Data.Logic;
using Microsoft.EntityFrameworkCore;

namespace DroneFlightLog.Data.Factory
{
    public class DroneFlightLogFactory<T> : IDroneFlightLogFactory<T> where T : DbContext, IDroneFlightLogDbContext
    {
        private Lazy<IFlightPropertyManager> _properties;
        private Lazy<IAddressManager> _addresses;
        private Lazy<IOperatorManager> _operators;
        private Lazy<IManufacturerManager> _manufacturers;
        private Lazy<IModelManager> _models;
        private Lazy<IDroneManager> _drones;
        private Lazy<ILocationManager> _locations;
        private Lazy<IFlightManager> _flights;

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
    }
}
