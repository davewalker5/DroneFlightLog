using System.Collections.Generic;
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
    public class ManufacturerManagerTests
    {
        private const string Name = "Some Drone Manufacturer";
        private const string UpdatedName = "Some Other Drone Manufacturer";
        private const string AsyncName = "Some Async Drone Manufacturer";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _manufacturerId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(Name);
            _factory.Context.SaveChanges();
            _manufacturerId = manufacturer.Id;
        }

        [TestMethod]
        public void AddManufacturerTest()
        {
            // The  manufacturer has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Manufacturers.Count());
            Assert.AreEqual(Name, _factory.Context.Manufacturers.First().Name);
        }

        [TestMethod]
        public async Task AddManufacturerAsyncTest()
        {
            Manufacturer manufacturer = await _factory.Manufacturers.AddManufacturerAsync(AsyncName);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Manufacturers.Count());
            Assert.AreEqual(AsyncName, manufacturer.Name);
        }

        [TestMethod, ExpectedException(typeof(ManufacturerExistsException))]
        public void AddExistingManufacturerTest()
        {
            _factory.Manufacturers.AddManufacturer(Name);
        }

        [TestMethod]
        public void GetManufacturerByIdTest()
        {
            Manufacturer manufacturer = _factory.Manufacturers.GetManufacturer(_manufacturerId);
            Assert.AreEqual(_manufacturerId, manufacturer.Id);
            Assert.AreEqual(Name, manufacturer.Name);
        }

        [TestMethod]
        public async Task GetManufacturerByIdAsyncTest()
        {
            Manufacturer manufacturer = await _factory.Manufacturers.GetManufacturerAsync(_manufacturerId);
            Assert.AreEqual(_manufacturerId, manufacturer.Id);
            Assert.AreEqual(Name, manufacturer.Name);
        }

        [TestMethod, ExpectedException(typeof(ManufacturerNotFoundException))]
        public void GetMissingManufacturerByIdTest()
        {
            _factory.Manufacturers.GetManufacturer(-1);
        }

        [TestMethod]
        public void UpdateManufacturerTest()
        {
            _factory.Manufacturers.UpdateManufacturer(_manufacturerId, UpdatedName);
            _factory.Context.SaveChanges();
            Manufacturer manufacturer = _factory.Manufacturers.GetManufacturer(_manufacturerId);
            Assert.AreEqual(_manufacturerId, manufacturer.Id);
            Assert.AreEqual(UpdatedName, manufacturer.Name);
        }

        [TestMethod]
        public async Task UpdateManufacturerAsyncTest()
        {
            await _factory.Manufacturers.UpdateManufacturerAsync(_manufacturerId, UpdatedName);
            await _factory.Context.SaveChangesAsync();
            Manufacturer manufacturer = await _factory.Manufacturers.GetManufacturerAsync(_manufacturerId);
            Assert.AreEqual(_manufacturerId, manufacturer.Id);
            Assert.AreEqual(UpdatedName, manufacturer.Name);
        }

        [TestMethod]
        public void GetAllManufacturersTest()
        {
            IEnumerable<Manufacturer> manufacturers = _factory.Manufacturers.GetManufacturers();
            Assert.AreEqual(1, manufacturers.Count());
            Assert.AreEqual(_manufacturerId, manufacturers.First().Id);
            Assert.AreEqual(Name, manufacturers.First().Name);
        }

        [TestMethod]
        public async Task GetAllManufacturersAsyncTest()
        {
            List<Manufacturer> manufacturers = await _factory.Manufacturers.GetManufacturersAsync().ToListAsync();
            Assert.AreEqual(1, manufacturers.Count());
            Assert.AreEqual(_manufacturerId, manufacturers.First().Id);
            Assert.AreEqual(Name, manufacturers.First().Name);
        }

        [TestMethod]
        public void FindManufacturerTest()
        {
            Manufacturer manufacturer = _factory.Manufacturers.FindManufacturer(Name);
            Assert.AreEqual(manufacturer.Name, Name);
        }

        [TestMethod]
        public async Task FindManufacturerAsyncTest()
        {
            Manufacturer manufacturer = await _factory.Manufacturers.FindManufacturerAsync(Name);
            Assert.AreEqual(manufacturer.Name, Name);
        }

        [TestMethod]
        public void FindMissingManufacturerTest()
        {
            Manufacturer manufacturer = _factory.Manufacturers.FindManufacturer("Missing");
            Assert.IsNull(manufacturer);
        }
    }
}
