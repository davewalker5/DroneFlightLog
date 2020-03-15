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
    public class AddressManagerTests
    {
        private const string Number = "1";
        private const string SecondNumber = "2";
        private const string Street = "Some Street";
        private const string Town = "Some Town";
        private const string County = "Some County";
        private const string Postcode = "AB12 3CD";
        private const string Country = "Some Country";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _addressId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Address address = _factory.Addresses.AddAddress(Number, Street, Town, County, Postcode, Country);
            _factory.Context.SaveChanges();
            _addressId = address.Id;
        }

        [TestMethod]
        public void AddAddressTest()
        {
            // The address has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Addresses.Count());
            Assert.AreEqual(Number, _factory.Context.Addresses.First().Number);
            Assert.AreEqual(Street, _factory.Context.Addresses.First().Street);
            Assert.AreEqual(Town, _factory.Context.Addresses.First().Town);
            Assert.AreEqual(County, _factory.Context.Addresses.First().County);
            Assert.AreEqual(Postcode, _factory.Context.Addresses.First().Postcode);
            Assert.AreEqual(Country, _factory.Context.Addresses.First().Country);
        }

        [TestMethod]
        public async Task AddAddressAsyncTest()
        {
            Address address = await _factory.Addresses.AddAddressAsync(SecondNumber, Street, Town, County, Postcode, Country);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Addresses.Count());
            Assert.AreNotEqual(_addressId, address.Id);
            Assert.AreEqual(SecondNumber, address.Number);
            Assert.AreEqual(Street, address.Street);
            Assert.AreEqual(Town, address.Town);
            Assert.AreEqual(County, address.County);
            Assert.AreEqual(Postcode, address.Postcode);
            Assert.AreEqual(Country, address.Country);
        }

        [TestMethod, ExpectedException(typeof(AddressExistsException))]
        public void AddExistingAddressTest()
        {
            _factory.Addresses.AddAddress(Number, "", "", "", Postcode, Country);
        }

        [TestMethod]
        public void GetAddressByIdTest()
        {
            Address address = _factory.Addresses.GetAddress(_addressId);
            Assert.AreEqual(_addressId, address.Id);
            Assert.AreEqual(Number, address.Number);
            Assert.AreEqual(Street, address.Street);
            Assert.AreEqual(Town, address.Town);
            Assert.AreEqual(County, address.County);
            Assert.AreEqual(Postcode, address.Postcode);
            Assert.AreEqual(Country, address.Country);
        }

        [TestMethod]
        public async Task GetAddressByIdAsyncTest()
        {
            Address address = await _factory.Addresses.GetAddressAsync(_addressId);
            Assert.AreEqual(_addressId, address.Id);
            Assert.AreEqual(Number, address.Number);
            Assert.AreEqual(Street, address.Street);
            Assert.AreEqual(Town, address.Town);
            Assert.AreEqual(County, address.County);
            Assert.AreEqual(Postcode, address.Postcode);
            Assert.AreEqual(Country, address.Country);
        }

        [TestMethod, ExpectedException(typeof(AddressNotFoundException))]
        public void GetMissingAddressByIdTest()
        {
            _factory.Addresses.GetAddress(-1);
        }

        [TestMethod]
        public void FindAddressTest()
        {
            Address address = _factory.Addresses.FindAddress(Number, Postcode, Country);
            Assert.AreEqual(_addressId, address.Id);
            Assert.AreEqual(Number, address.Number);
            Assert.AreEqual(Street, address.Street);
            Assert.AreEqual(Town, address.Town);
            Assert.AreEqual(County, address.County);
            Assert.AreEqual(Postcode, address.Postcode);
            Assert.AreEqual(Country, address.Country);
        }

        [TestMethod]
        public async Task FindAddressAsyncTest()
        {
            Address address = await _factory.Addresses.FindAddressAsync(Number, Postcode, Country);
            Assert.AreEqual(_addressId, address.Id);
            Assert.AreEqual(Number, address.Number);
            Assert.AreEqual(Street, address.Street);
            Assert.AreEqual(Town, address.Town);
            Assert.AreEqual(County, address.County);
            Assert.AreEqual(Postcode, address.Postcode);
            Assert.AreEqual(Country, address.Country);
        }

        [TestMethod]
        public void FindMissingAddressTest()
        {
            Address address = _factory.Addresses.FindAddress("Missing", "Missing", "Missing");
            Assert.IsNull(address);
        }
    }
}
