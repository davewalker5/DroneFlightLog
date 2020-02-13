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
    public class LocationManagerTests
    {
        private const string Name = "My Local Drone Flight Location";
        private const string AsyncName = "My Local Async Drone Flight Location";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _locationId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Location location = _factory.Locations.AddLocation(Name);
            _factory.Context.SaveChanges();
            _locationId = location.Id;
        }

        [TestMethod]
        public void AddLocationTest()
        {
            // The location has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Locations.Count());
            Assert.AreEqual(Name, _factory.Context.Locations.First().Name);
        }

        [TestMethod]
        public async void AddLocationAsyncTest()
        {
            Location location = _factory.Locations.AddLocation(AsyncName);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Locations.Count());
            Assert.AreEqual(AsyncName, location.Name);
        }

        [TestMethod, ExpectedException(typeof(LocationExistsException))]
        public void AddExistingLocationTest()
        {
            _factory.Locations.AddLocation(Name);
        }

        [TestMethod]
        public void GetLocationByIdTest()
        {
            Location location = _factory.Locations.GetLocation(_locationId);
            Assert.AreEqual(_locationId, location.Id);
            Assert.AreEqual(Name, location.Name);
        }

        [TestMethod]
        public async void GetLocationByIdAsyncTest()
        {
            Location location = await _factory.Locations.GetLocationAsync(_locationId);
            Assert.AreEqual(_locationId, location.Id);
            Assert.AreEqual(Name, location.Name);
        }

        [TestMethod, ExpectedException(typeof(LocationNotFoundException))]
        public void GetMissingLocationByIdTest()
        {
            _factory.Locations.GetLocation(-1);
        }

        [TestMethod]
        public void GetAllLocationsTest()
        {
            IEnumerable<Location> locations = _factory.Locations.GetLocations();
            Assert.AreEqual(1, locations.Count());
            int locationId = _factory.Context.Locations.First().Id;
            Assert.AreEqual(locationId, locations.First().Id);
            Assert.AreEqual(Name, locations.First().Name);
        }

        [TestMethod]
        public async void GetAllLocationsAsyncTest()
        {
            List<Location> locations = await _factory.Locations.GetLocationsAsync().ToListAsync();
            Assert.AreEqual(1, locations.Count());
            int locationId = _factory.Context.Locations.First().Id;
            Assert.AreEqual(locationId, locations.First().Id);
            Assert.AreEqual(Name, locations.First().Name);
        }

        [TestMethod]
        public void FindLocationTest()
        {
            Location location = _factory.Locations.FindLocation(Name);
            Assert.AreEqual(location.Name, Name);
        }

        [TestMethod]
        public async void FindLocationAsyncTest()
        {
            Location location = await _factory.Locations.FindLocationAsync(Name);
            Assert.AreEqual(location.Name, Name);
        }

        [TestMethod]
        public void FindMissingLocationTest()
        {
            Location location = _factory.Locations.FindLocation("Missing");
            Assert.IsNull(location);
        }
    }
}
