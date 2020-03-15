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
    public class FlightPropertyManagerTest
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

        private const string PropertyName = "Wind Speed";
        private const FlightPropertyDataType PropertyType = FlightPropertyDataType.Number;
        private const decimal PropertyValue = 7.8M;

        private const string MultiInstancePropertyName = "Some Property";
        private const string DatePropertyName = "Some Date Property";
        private const string StringPropertyName = "Some String Property";
        private const string StringPropertyValue = "Some Value";

        private const string AsyncPropertyName = "Some Async Property";
        private const decimal AsyncPropertyValue = 6.45M;

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _flightId;
        private int _propertyId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Address address = _factory.Addresses.AddAddress(Number, Street, Town, County, Postcode, Country);
            _factory.Context.SaveChanges();

            Operator op = _factory.Operators.AddOperator(FirstNames, Surname, DoB, FlyerNumber, OperatorNumber, address.Id);
            _factory.Context.SaveChanges();

            Location location = _factory.Locations.AddLocation(LocationName);
            _factory.Context.SaveChanges();

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();

            Model model = _factory.Models.AddModel(ModelName, manufacturer.Id);
            _factory.Context.SaveChanges();

            Drone drone = _factory.Drones.AddDrone(DroneName, DroneSerialNumber, model.Id);
            _factory.Context.SaveChanges();

            Flight flight = _factory.Flights.AddFlight(op.Id, drone.Id, location.Id, DateTime.Now, DateTime.Now);
            _factory.Context.SaveChanges();
            _flightId = flight.Id;

            FlightProperty property = _factory.Properties.AddProperty(PropertyName, PropertyType, true);
            _factory.Context.SaveChanges();
            _propertyId = property.Id;

            _factory.Properties.AddPropertyValue(_flightId, _propertyId, PropertyValue);
            _factory.Context.SaveChanges();
        }

        [TestMethod]
        public void AddPropertyTest()
        {
            // The property has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.FlightProperties.Count());
            Assert.AreEqual(PropertyName, _factory.Context.FlightProperties.First().Name);
            Assert.AreEqual(PropertyType, _factory.Context.FlightProperties.First().DataType);
            Assert.IsTrue(_factory.Context.FlightProperties.First().IsSingleInstance);
        }

        [TestMethod]
        public async Task AddPropertyAsyncTest()
        {
            FlightProperty property = await _factory.Properties.AddPropertyAsync(AsyncPropertyName, PropertyType, true);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.FlightProperties.Count());
            Assert.AreEqual(AsyncPropertyName, property.Name);
            Assert.AreEqual(PropertyType, property.DataType);
            Assert.IsTrue(property.IsSingleInstance);
        }

        [TestMethod, ExpectedException(typeof(PropertyExistsException))]
        public void AddExistingPropertyTest()
        {
            _factory.Properties.AddProperty(PropertyName, PropertyType, true);
        }

        [TestMethod]
        public void GetPropertiesTest()
        {
            IEnumerable<FlightProperty> properties = _factory.Properties.GetProperties();
            Assert.AreEqual(1, properties.Count());
            Assert.AreEqual(PropertyName, properties.First().Name);
            Assert.AreEqual(PropertyType, properties.First().DataType);
            Assert.IsTrue(properties.First().IsSingleInstance);
        }

        [TestMethod]
        public async Task GetPropertiesAsyncTest()
        {
            List<FlightProperty> properties = await _factory.Properties.GetPropertiesAsync().ToListAsync();
            Assert.AreEqual(1, properties.Count());
            Assert.AreEqual(PropertyName, properties.First().Name);
            Assert.AreEqual(PropertyType, properties.First().DataType);
            Assert.IsTrue(properties.First().IsSingleInstance);
        }

        [TestMethod]
        public void AddPropertyValueTest()
        {
            // The property has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.FlightPropertyValues.Count());
            Assert.AreEqual(_flightId, _factory.Context.FlightPropertyValues.First().FlightId);
            Assert.AreEqual(_propertyId, _factory.Context.FlightPropertyValues.First().PropertyId);
            Assert.AreEqual(PropertyValue, _factory.Context.FlightPropertyValues.First().NumberValue);
        }

        [TestMethod]
        public async Task AddPropertyValueAsyncTest()
        {
            // To allow this to work, we need to make the existing property multi-value
            _factory.Context.FlightProperties.First(p => p.Id == _propertyId).IsSingleInstance = false;
            await _factory.Context.SaveChangesAsync();

            FlightPropertyValue value = await _factory.Properties.AddPropertyValueAsync(_flightId, _propertyId, AsyncPropertyValue);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.FlightPropertyValues.Count());
            Assert.AreEqual(_flightId, value.FlightId);
            Assert.AreEqual(_propertyId, value.PropertyId);
            Assert.AreEqual(AsyncPropertyValue, value.NumberValue);
        }

        [TestMethod, ExpectedException(typeof(ValueExistsException))]
        public void AddDuplicateSingleInstanceValueTest()
        {
            _factory.Properties.AddPropertyValue(_flightId, _propertyId, PropertyValue);
        }

        [TestMethod]
        public void GetPropertyValuesTest()
        {
            IEnumerable<FlightPropertyValue> values = _factory.Properties.GetPropertyValues(_flightId);
            Assert.AreEqual(1, values.Count());
            Assert.AreEqual(_flightId, values.First().FlightId);
            Assert.AreEqual(_propertyId, values.First().PropertyId);
            Assert.AreEqual(PropertyValue, values.First().NumberValue);
        }

        [TestMethod]
        public async Task GetPropertyValuesAsyncTest()
        {
            List<FlightPropertyValue> values = await _factory.Properties.GetPropertyValuesAsync(_flightId).ToListAsync();
            Assert.AreEqual(1, values.Count());
            Assert.AreEqual(_flightId, values.First().FlightId);
            Assert.AreEqual(_propertyId, values.First().PropertyId);
            Assert.AreEqual(PropertyValue, values.First().NumberValue);
        }

        [TestMethod]
        public void GetPropertyValuesForMissingFlightTest()
        {
            IEnumerable<FlightPropertyValue> values = _factory.Properties.GetPropertyValues(-1);
            Assert.IsFalse(values.Any());
        }

        [TestMethod]
        public void AddMultiInstancePropertyValuesTest()
        {
            FlightProperty property = _factory.Properties.AddProperty(MultiInstancePropertyName, PropertyType, false);
            _factory.Context.SaveChanges();

            _factory.Properties.AddPropertyValue(_flightId, property.Id, 1.0M);
            _factory.Properties.AddPropertyValue(_flightId, property.Id, 2.0M);
            _factory.Context.SaveChanges();

            IEnumerable<FlightPropertyValue> values = _factory.Properties.GetPropertyValues(_flightId);
            Assert.AreEqual(3, values.Count());
        }

        [TestMethod]
        public void AddDatePropertyValueTest()
        {
            FlightProperty property = _factory.Properties.AddProperty(DatePropertyName, FlightPropertyDataType.Date, true);
            _factory.Context.SaveChanges();

            DateTime date = DateTime.Now;
            _factory.Properties.AddPropertyValue(_flightId, property.Id, date);
            _factory.Context.SaveChanges();

            FlightPropertyValue value = _factory.Properties.GetPropertyValues(_flightId).First(v => v.PropertyId == property.Id);
            Assert.AreEqual(value.DateValue, date);
        }

        [TestMethod]
        public void AddStringPropertyValueTest()
        {
            FlightProperty property = _factory.Properties.AddProperty(StringPropertyName, FlightPropertyDataType.String, true);
            _factory.Context.SaveChanges();

            _factory.Properties.AddPropertyValue(_flightId, property.Id, StringPropertyValue);
            _factory.Context.SaveChanges();

            FlightPropertyValue value = _factory.Properties.GetPropertyValues(_flightId).First(v => v.PropertyId == property.Id);
            Assert.AreEqual(value.StringValue, StringPropertyValue);
        }
    }
}
