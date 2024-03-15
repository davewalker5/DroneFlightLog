using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.InMemory;
using DroneFlightLog.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DroneFlightLog.Data.Tests
{
    [TestClass]
    public class MaintainerManagerTests
    {
        private const string AsyncFirstNames = "Async First Names";
        private const string FirstNames = "First Names";
        private const string UpdatedFirstNames = "Updated First Names";
        private const string Surname = "Surname";
        private const string UpdatedSurname = "Updated Surname";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _maintainerId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Maintainer maintainer = _factory.Maintainers.AddMaintainer(FirstNames, Surname);
            _factory.Context.SaveChanges();
            _maintainerId = maintainer.Id;
        }

        [TestMethod]
        public void AddMaintainerTest()
        {
            // The maintainer has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Maintainers.Count());
            Assert.AreEqual(FirstNames, _factory.Context.Maintainers.First().FirstNames);
            Assert.AreEqual(Surname, _factory.Context.Maintainers.First().Surname);
        }

        [TestMethod]
        public async Task AddMaintainerAsyncTest()
        {
            Maintainer maintainer = await _factory.Maintainers.AddMaintainerAsync(AsyncFirstNames, Surname);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Maintainers.Count());
            Assert.AreEqual(AsyncFirstNames, maintainer.FirstNames);
            Assert.AreEqual(Surname, maintainer.Surname);
        }

        [TestMethod, ExpectedException(typeof(MaintainerExistsException))]
        public void AddExistingMaintainerTest()
        {
            _factory.Maintainers.AddMaintainer(FirstNames, Surname);
        }

        [TestMethod]
        public void UpdateMaintainerTest()
        {
            _factory.Maintainers.UpdateMaintainer(_maintainerId, UpdatedFirstNames, UpdatedSurname);
            _factory.Context.SaveChanges();
            Maintainer maintainer = _factory.Maintainers.GetMaintainer(_maintainerId);
            Assert.AreEqual(UpdatedFirstNames, maintainer.FirstNames);
            Assert.AreEqual(UpdatedSurname, maintainer.Surname);
        }

        [TestMethod]
        public async Task UpdateMaintainerAsyncTest()
        {
            await _factory.Maintainers.UpdateMaintainerAsync(_maintainerId, UpdatedFirstNames, UpdatedSurname);
            await _factory.Context.SaveChangesAsync();
            Maintainer maintainer = await _factory.Maintainers.GetMaintainerAsync(_maintainerId);
            Assert.AreEqual(UpdatedFirstNames, maintainer.FirstNames);
            Assert.AreEqual(UpdatedSurname, maintainer.Surname);
        }

        [TestMethod]
        public void GetMaintainerByIdTest()
        {
            Maintainer maintainer = _factory.Maintainers.GetMaintainer(_maintainerId);
            ValidateMaintainer(maintainer, _maintainerId);
        }

        [TestMethod]
        public async Task GetMaintainerByIdAsyncTest()
        {
            Maintainer maintainer = await _factory.Maintainers.GetMaintainerAsync(_maintainerId);
            ValidateMaintainer(maintainer, _maintainerId);
        }

        [TestMethod, ExpectedException(typeof(MaintainerNotFoundException))]
        public void GetMissingMaintainerByIdTest()
        {
            _factory.Maintainers.GetMaintainer(-1);
        }

        [TestMethod]
        public void GetAllMaintainersTest()
        {
            IEnumerable<Maintainer> maintainers = _factory.Maintainers.GetMaintainers();
            Assert.AreEqual(1, maintainers.Count());
            ValidateMaintainer(maintainers.First(), _maintainerId);
        }

        [TestMethod]
        public async Task GetAllMaintainersAsyncTest()
        {
            List<Maintainer> maintainers = await _factory.Maintainers.GetMaintainersAsync().ToListAsync();
            Assert.AreEqual(1, maintainers.Count());
            ValidateMaintainer(maintainers.First(), _maintainerId);
        }

        [TestMethod]
        public void FindMaintainerTest()
        {
            Maintainer maintainer = _factory.Maintainers.FindMaintainer(FirstNames, Surname);
            ValidateMaintainer(maintainer, _maintainerId);
        }

        [TestMethod]
        public async Task FindMaintainerAsyncTest()
        {
            Maintainer maintainer = await _factory.Maintainers.FindMaintainerAsync(FirstNames, Surname);
            ValidateMaintainer(maintainer, _maintainerId);
        }

        [TestMethod]
        public void FindMissingMaintainerTest()
        {
            Maintainer maintainer = _factory.Maintainers.FindMaintainer("", "");
            Assert.IsNull(maintainer);
        }

        #region Helpers
        /// <summary>
        /// Validate the specified maintainer
        /// </summary>
        /// <param name="maintainer"></param>
        /// <param name="expectedMaintainerId"></param>
        /// <param name="expectedAddressId"></param>
        private void ValidateMaintainer(Maintainer maintainer, int expectedMaintainerId)
        {
            Assert.AreEqual(expectedMaintainerId, maintainer.Id);
            Assert.AreEqual(FirstNames, maintainer.FirstNames);
            Assert.AreEqual(Surname, maintainer.Surname);
        }
        #endregion
    }
}
