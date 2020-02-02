using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.InMemory;
using DroneFlightLog.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DroneFlightLog.Data.Tests
{
    [TestClass]
    public class DroneManagerTest
    {
        private const string ManufacturerName = "Some Manufacturer";
        private const string ModelName = "Some Model";
        private const string DroneName = "Some Drone";
        private const string DroneSerialNumber = "1234567890";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _modelId;
        private int _droneId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();

            Model model = _factory.Models.AddModel(ModelName, manufacturer.Id);
            _factory.Context.SaveChanges();
            _modelId = model.Id;

            Drone drone = _factory.Drones.AddDrone(DroneName, DroneSerialNumber, _modelId);
            _factory.Context.SaveChanges();
            _droneId = drone.Id;
        }

        [TestMethod]
        public void AddDroneTest()
        {
            // The  drone has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Drones.Count());
            Assert.AreEqual(DroneName, _factory.Context.Drones.First().Name);
            Assert.AreEqual(DroneSerialNumber, _factory.Context.Drones.First().SerialNumber);
            Assert.AreEqual(_modelId, _factory.Context.Drones.First().ModelId);
        }

        [TestMethod, ExpectedException(typeof(DroneExistsException))]
        public void AddExistingDroneTest()
        {
            _factory.Drones.AddDrone(DroneName, DroneSerialNumber, _modelId);
        }

        [TestMethod, ExpectedException(typeof(ModelNotFoundException))]
        public void AddDroneForMissingModelTest()
        {
            _factory.Drones.AddDrone("", "", -1);
        }

        [TestMethod]
        public void GetDroneByIdTest()
        {
            Drone drone = _factory.Drones.GetDrone(_droneId);
            ValidateDrone(drone);
        }

        [TestMethod, ExpectedException(typeof(DroneNotFoundException))]
        public void GetMissingDroneByIdTest()
        {
            _factory.Drones.GetDrone(-1);
        }

        [TestMethod]
        public void GetAllDronesTest()
        {
            IEnumerable<Drone> drones = _factory.Drones.GetDrones(null);
            Assert.AreEqual(1, drones.Count());
            ValidateDrone(drones.First());
        }

        [TestMethod]
        public void GetDronesByModelTest()
        {
            IEnumerable<Drone> drones = _factory.Drones.GetDrones(_modelId);
            Assert.AreEqual(1, drones.Count());
            ValidateDrone(drones.First());
        }

        [TestMethod]
        public void GetDronesByMissingModelTest()
        {
            IEnumerable<Drone> drones = _factory.Drones.GetDrones(-1);
            Assert.IsFalse(drones.Any());
        }

        [TestMethod]
        public void FindDroneTest()
        {
            Drone drone = _factory.Drones.FindDrone(DroneSerialNumber, _modelId);
            ValidateDrone(drone);
        }

        [TestMethod]
        public void FindDroneByMissingSerialNumberTest()
        {
            Drone drone = _factory.Drones.FindDrone("", _modelId);
            Assert.IsNull(drone);
        }

        [TestMethod]
        public void FindDroneByMissingModelTest()
        {
            Drone drone = _factory.Drones.FindDrone(DroneSerialNumber, -1);
            Assert.IsNull(drone);
        }

        #region Helpers
        /// <summary>
        /// Check the properties of a drone
        /// </summary>
        /// <param name="drone"></param>
        private void ValidateDrone(Drone drone)
        {
            Assert.AreEqual(_droneId, drone.Id);
            Assert.AreEqual(DroneName, drone.Name);
            Assert.AreEqual(DroneSerialNumber, drone.SerialNumber);
            Assert.AreEqual(_modelId, drone.ModelId);
        }
        #endregion
    }
}
