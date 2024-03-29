﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.InMemory;
using DroneFlightLog.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DroneFlightLog.Data.Tests
{
    [TestClass]
    public class DroneManagerTests
    {
        private const string ManufacturerName = "Some Manufacturer";
        private const string ModelName = "Some Model";
        private const string UpdatedModelName = "Some Other Model";
        private const string DroneName = "Some Drone";
        private const string UpdatedDroneName = "Some Other Drone";
        private const string DroneSerialNumber = "1234567890";
        private const string UpdatedDroneSerialNumber = "ABCDEFGHIJ";
        private const string SecondDroneSerialNumber = "0987654321";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _manufacturerId;
        private int _modelId;
        private int _droneId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();
            _manufacturerId = manufacturer.Id;

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

        [TestMethod]
        public async Task AddDroneAsyncTest()
        {
            Drone drone = await _factory.Drones.AddDroneAsync(DroneName, SecondDroneSerialNumber, _modelId);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Drones.Count());
            Assert.AreNotEqual(_droneId, drone.Id);
            Assert.AreEqual(DroneName, drone.Name);
            Assert.AreEqual(SecondDroneSerialNumber, drone.SerialNumber);
            Assert.AreEqual(_modelId, drone.ModelId);
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

        [TestMethod]
        public async Task GetDroneByIdAsyncTest()
        {
            Drone drone = await _factory.Drones.GetDroneAsync(_droneId);
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
        public async Task GetAllDronesAsyncTest()
        {
            List<Drone> drones = await _factory.Drones.GetDronesAsync(null).ToListAsync();
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
        public async Task GetDronesByModelAsyncTest()
        {
            List<Drone> drones = await _factory.Drones.GetDronesAsync(_modelId).ToListAsync();
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
        public void UpdateDroneTest()
        {
            // To make this a complete test, we need a second model
            Model model  = _factory.Models.AddModel(UpdatedModelName, _manufacturerId);
            _factory.Context.SaveChanges();

            _factory.Drones.UpdateDrone(_droneId, UpdatedDroneName, UpdatedDroneSerialNumber, model.Id);
            _factory.Context.SaveChanges();

            Drone drone = _factory.Drones.GetDrone(_droneId);
            Assert.AreEqual(UpdatedDroneName, drone.Name);
            Assert.AreEqual(UpdatedDroneSerialNumber, drone.SerialNumber);
            Assert.AreEqual(model.Id, drone.ModelId);
        }

        [TestMethod]
        public async Task UpdateDroneTestAsync()
        {
            // To make this a complete test, we need a second model
            Model model = await _factory.Models.AddModelAsync(UpdatedModelName, _manufacturerId);
            await _factory.Context.SaveChangesAsync();

            await _factory.Drones.UpdateDroneAsync(_droneId, UpdatedDroneName, UpdatedDroneSerialNumber, model.Id);
            await _factory.Context.SaveChangesAsync();

            Drone drone = await _factory.Drones.GetDroneAsync(_droneId);
            Assert.AreEqual(UpdatedDroneName, drone.Name);
            Assert.AreEqual(UpdatedDroneSerialNumber, drone.SerialNumber);
            Assert.AreEqual(model.Id, drone.ModelId);
        }

        [TestMethod]
        public void FindDroneTest()
        {
            Drone drone = _factory.Drones.FindDrone(DroneSerialNumber, _modelId);
            ValidateDrone(drone);
        }

        [TestMethod]
        public async Task FindDroneAsyncTest()
        {
            Drone drone = await _factory.Drones.FindDroneAsync(DroneSerialNumber, _modelId);
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
