using System;
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
    public class FlightManagerTest
    {
        private const string Number = "1";
        private const string Street = "Some Street";
        private const string Town = "Some Town";
        private const string County = "Some County";
        private const string Postcode = "AB12 3CD";
        private const string Country = "Some Country";

        private const string FirstNames = "First Names";
        private const string SecondFirstNames = "Second Operator First Names";
        private const string Surname = "Surname";
        private readonly DateTime DoB = DateTime.Now;
        private const string FlyerNumber = "Some Flyer Number";
        private const string OperatorNumber = "Some Operator Number";

        private const string LocationName = "My Local Drone Flight Location";
        private const string SecondLocationName = "My Second Local Drone Flight Location";

        private const string ManufacturerName = "Some Manufacturer";
        private const string ModelName = "Some Model";
        private const string DroneName = "Some Drone";
        private const string SecondDroneName = "Some Other Drone";
        private const string DroneSerialNumber = "1234567890";
        private const string SecondDroneSerialNumber = "0987654321";

        private readonly DateTime StartDate = DateTime.Now.AddMinutes(-5);
        private readonly DateTime EndDate = DateTime.Now.AddMinutes(5);

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _operatorId;
        private int _secondOperatorId;
        private int _locationId;
        private int _secondLocationId;
        private int _droneId;
        private int _secondDroneId;
        private int _flightId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Address address = _factory.Addresses.AddAddress(Number, Street, Town, County, Postcode, Country);
            _factory.Context.SaveChanges();

            Operator op = _factory.Operators.AddOperator(FirstNames, Surname, DoB, FlyerNumber, OperatorNumber, address.Id);
            Operator secondOp = _factory.Operators.AddOperator(SecondFirstNames, Surname, DoB, FlyerNumber, OperatorNumber, address.Id);
            _factory.Context.SaveChanges();
            _operatorId = op.Id;
            _secondOperatorId = secondOp.Id;

            Location location = _factory.Locations.AddLocation(LocationName);
            Location secondLocation = _factory.Locations.AddLocation(SecondLocationName);
            _factory.Context.SaveChanges();
            _locationId = location.Id;
            _secondLocationId = secondLocation.Id;

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();

            Model model = _factory.Models.AddModel(ModelName, manufacturer.Id);
            _factory.Context.SaveChanges();

            Drone drone = _factory.Drones.AddDrone(DroneName, DroneSerialNumber, model.Id);
            Drone secondDrone = _factory.Drones.AddDrone(SecondDroneName, SecondDroneSerialNumber, model.Id);
            _factory.Context.SaveChanges();
            _droneId = drone.Id;
            _secondDroneId = secondDrone.Id;

            Flight flight = _factory.Flights.AddFlight(op.Id, drone.Id, location.Id, StartDate, EndDate);
            _factory.Context.SaveChanges();
            _flightId = flight.Id;
        }

        [TestMethod]
        public void GetFlightTest()
        {
            Flight flight = _factory.Flights.GetFlight(_flightId);
            Assert.AreEqual(_flightId, flight.Id);
            Assert.AreEqual(_droneId, flight.DroneId);
            Assert.AreEqual(_locationId, flight.LocationId);
            Assert.AreEqual(_operatorId, flight.OperatorId);
            Assert.AreEqual(StartDate, flight.Start);
            Assert.AreEqual(EndDate, flight.End);
        }

        [TestMethod]
        public async Task GetFlightAsyncTest()
        {
            Flight flight = await _factory.Flights.GetFlightAsync(_flightId);
            Assert.AreEqual(_flightId, flight.Id);
            Assert.AreEqual(_droneId, flight.DroneId);
            Assert.AreEqual(_locationId, flight.LocationId);
            Assert.AreEqual(_operatorId, flight.OperatorId);
            Assert.AreEqual(StartDate, flight.Start);
            Assert.AreEqual(EndDate, flight.End);
        }

        [TestMethod, ExpectedException(typeof(FlightNotFoundException))]
        public void GetMissingFlightByIdTest()
        {
            _factory.Flights.GetFlight(-1);
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
            Assert.AreEqual(StartDate, _factory.Context.Flights.First().Start);
            Assert.AreEqual(EndDate, _factory.Context.Flights.First().End);
        }

        [TestMethod]
        public async Task AddFlightAsyncTest()
        {
            Flight flight = await _factory.Flights.AddFlightAsync(_operatorId, _droneId, _locationId, StartDate, EndDate);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Flights.Count());
            Assert.AreEqual(_droneId, flight.DroneId);
            Assert.AreEqual(_locationId, flight.LocationId);
            Assert.AreEqual(_operatorId, flight.OperatorId);
            Assert.AreEqual(StartDate, flight.Start);
            Assert.AreEqual(EndDate, flight.End);
        }

        [TestMethod]
        public void UpdateFlightTest()
        {
            DateTime start = StartDate.AddDays(-1);
            DateTime end = EndDate.AddDays(-1);
            _factory.Flights.UpdateFlight(_flightId, _secondOperatorId, _secondDroneId, _secondLocationId, start, end);
            _factory.Context.SaveChanges();

            Flight flight = _factory.Flights.GetFlight(_flightId);
            Assert.AreEqual(_secondDroneId, flight.DroneId);
            Assert.AreEqual(_secondLocationId, flight.LocationId);
            Assert.AreEqual(_secondOperatorId, flight.OperatorId);
            Assert.AreEqual(start, flight.Start);
            Assert.AreEqual(end, flight.End);
        }

        [TestMethod]
        public async Task UpdateFlightAsyncTest()
        {
            DateTime start = StartDate.AddDays(-1);
            DateTime end = EndDate.AddDays(-1);
            await _factory.Flights.UpdateFlightAsync(_flightId, _secondOperatorId, _secondDroneId, _secondLocationId, start, end);
            await _factory.Context.SaveChangesAsync();

            Flight flight = await _factory.Flights.GetFlightAsync(_flightId);
            Assert.AreEqual(_secondDroneId, flight.DroneId);
            Assert.AreEqual(_secondLocationId, flight.LocationId);
            Assert.AreEqual(_secondOperatorId, flight.OperatorId);
            Assert.AreEqual(start, flight.Start);
            Assert.AreEqual(end, flight.End);
        }

        [TestMethod]
        public void FindFlightByOperatorIdTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.FindFlights(_operatorId, null, null, null, null, 1, 2);
            Assert.AreEqual(1, flights.Count());
            ValidateFlight(flights.First());
        }

        [TestMethod]
        public async Task FindFlightByOperatorIdAsyncTest()
        {
            List<Flight> flights = await _factory.Flights.FindFlightsAsync(_operatorId, null, null, null, null, 1, 2).ToListAsync();
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
