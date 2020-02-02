using System;
using System.Collections.Generic;
using System.Linq;
using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.InMemory;
using DroneFlightLog.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DroneFlightLog.Data.Tests
{
    [TestClass]
    public class FlightManagerTest
    {
        private const string Number = "1";
        private const string Street = "Some Street";
        private const string Town = "Some Town";
        private const string County = "Some County";
        private const string Postcode = "AB12 3CD";
        private const string Country = "Some Country";

        private const string FirstNames = "First Names";
        private const string Surname = "Surname";
        private readonly DateTime DoB = DateTime.Now;
        private const string FlyerNumber = "Some Flyer Number";
        private const string OperatorNumber = "Some Operator Number";

        private const string LocationName = "My Local Drone Flight Location";

        private const string ManufacturerName = "Some Manufacturer";
        private const string ModelName = "Some Model";
        private const string DroneName = "Some Drone";
        private const string DroneSerialNumber = "1234567890";

        private readonly DateTime StartDate = DateTime.Now.AddMinutes(-5);
        private readonly DateTime EndDate = DateTime.Now.AddMinutes(5);

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _operatorId;
        private int _locationId;
        private int _droneId;
        private int _flightId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Address address = _factory.Addresses.AddAddress(Number, Street, Town, County, Postcode, Country);
            _factory.Context.SaveChanges();

            Operator op = _factory.Operators.AddOperator(FirstNames, Surname, DoB, FlyerNumber, OperatorNumber, address.Id);
            _factory.Context.SaveChanges();
            _operatorId = op.Id;

            Location location = _factory.Locations.AddLocation(LocationName);
            _factory.Context.SaveChanges();
            _locationId = location.Id;

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();

            Model model = _factory.Models.AddModel(ModelName, manufacturer.Id);
            _factory.Context.SaveChanges();

            Drone drone = _factory.Drones.AddDrone(DroneName, DroneSerialNumber, model.Id);
            _factory.Context.SaveChanges();
            _droneId = drone.Id;

            Flight flight = _factory.Flights.AddFlight(op.Id, drone.Id, location.Id, StartDate, EndDate);
            _factory.Context.SaveChanges();
            _flightId = flight.Id;
        }

        [TestMethod]
        public void AddFlightTest()
        {
            // The flight has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Flights.Count());
            Assert.AreEqual(_flightId, _factory.Context.Flights.First().Id);
            Assert.AreEqual(_droneId, _factory.Context.Flights.First().DroneId);
            Assert.AreEqual(_locationId, _factory.Context.Flights.First().LocationId);
            Assert.AreEqual(_operatorId, _factory.Context.Flights.First().OperatorId);
        }

        [TestMethod]
        public void FindFlightByOperatorIdTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(_operatorId, null, null, null, null, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        [TestMethod]
        public void FindFlightByDroneIdTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(null, _droneId, null, null, null, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        [TestMethod]
        public void FindFlightByLocationIdTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(null, null, _locationId, null, null, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        [TestMethod]
        public void FindFlightByStartDateTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(null, null, null, StartDate, null, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        [TestMethod]
        public void FindFlightByEndDateTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(null, null, null, null, EndDate, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        [TestMethod]
        public void FindFlightByMultipleCriteriaTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(_operatorId, _droneId, _locationId, StartDate, EndDate, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        #region Helpers
        /// <summary>
        /// Validate the properties of the specified flight
        /// </summary>
        /// <param name="flight"></param>
        private void ValidateFlight(Flight flight)
        {
            Assert.AreEqual(_flightId, flight.Id);
            Assert.AreEqual(_droneId, flight.DroneId);
            Assert.AreEqual(_locationId, flight.LocationId);
            Assert.AreEqual(_operatorId, flight.OperatorId);
        }
        #endregion
    }
}
