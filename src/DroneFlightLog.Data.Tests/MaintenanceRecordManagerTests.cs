using DroneFlightLog.Data.Entities;
using DroneFlightLog.Data.Exceptions;
using DroneFlightLog.Data.Factory;
using DroneFlightLog.Data.InMemory;
using DroneFlightLog.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneFlightLog.Data.Tests
{
    [TestClass]
    public class MaintenanceRecordManagerTests
    {
        private const string MaintainerFirstNames = "A";
        private const string SecondMaintainerFirstNames = "Another";
        private const string MaintainerSurname = "Maintainer";

        private const string MaintenanceRecordDescription = "Some Maintenance Work";
        private const string MaintenanceRecordNotes = "Did some maintenance work";

        private const string ModificationRecordDescription = "A Modification";
        private const string ModificationRecordNotes = "Did some modification work";

        private const string ManufacturerName = "Holy Stone";
        private const string ModelName = "HS165";
        private const string DroneName = "My Drone";
        private const string DroneSerialNumber = "0123456789";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _droneId;
        private int _firstMaintainerId;
        private int _secondMaintainerId;
        private int _maintenanceRecordId;

        [TestInitialize]
        public void TestInitialise()
        {
            var context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            var manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();

            var model = _factory.Models.AddModel(ModelName, manufacturer.Id);
            _factory.Context.SaveChanges();

            var drone = _factory.Drones.AddDrone(DroneName, DroneSerialNumber, model.Id);
            _factory.Context.SaveChanges();
            _droneId = drone.Id;

            var maintainer = _factory.Maintainers.AddMaintainer(MaintainerFirstNames, MaintainerSurname);
            _factory.Context.SaveChanges();
            _firstMaintainerId = maintainer.Id;

            maintainer = _factory.Maintainers.AddMaintainer(SecondMaintainerFirstNames, MaintainerSurname);
            _factory.Context.SaveChanges();
            _secondMaintainerId = maintainer.Id;

            var maintenanceRecord = _factory.MaintenanceRecords.AddMaintenanceRecord(_firstMaintainerId, _droneId, MaintenanceRecordType.Maintenance, DateTime.Now, MaintenanceRecordDescription, MaintenanceRecordNotes);
            _factory.Context.SaveChanges();
            _maintenanceRecordId = maintenanceRecord.Id;
        }

        [TestMethod]
        public void AddMaintenanceRecordTest()
        {
            // The maintainer has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.MaintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, _factory.Context.MaintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, _factory.Context.MaintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, _factory.Context.MaintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, _factory.Context.MaintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, _factory.Context.MaintenanceRecords.First().Notes);
        }

        [TestMethod]
        public async Task AddMaintenanceRecordAsyncTest()
        {
            var maintenanceRecord = await _factory.MaintenanceRecords.AddMaintenanceRecordAsync(_secondMaintainerId, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
            await _factory.Context.SaveChangesAsync();

            Assert.AreEqual(2, _factory.Context.MaintenanceRecords.Count());
            Assert.AreEqual(_secondMaintainerId, maintenanceRecord.MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecord.DroneId);
            Assert.AreEqual(MaintenanceRecordType.Modification, maintenanceRecord.RecordType);
            Assert.AreEqual(ModificationRecordDescription, maintenanceRecord.Description);
            Assert.AreEqual(ModificationRecordNotes, maintenanceRecord.Notes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintainerNotFoundException))]
        public void AddMaintenanceRecordInvalidMaintainerTest()
        {
            _factory.MaintenanceRecords.AddMaintenanceRecord(0, _droneId, MaintenanceRecordType.Maintenance, DateTime.Now, MaintenanceRecordDescription, MaintenanceRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintainerNotFoundException))]
        public async Task AddMaintenanceRecordInvalidMaintainerAsyncTest()
        {
            await _factory.MaintenanceRecords.AddMaintenanceRecordAsync(0, _droneId, MaintenanceRecordType.Maintenance, DateTime.Now, MaintenanceRecordDescription, MaintenanceRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(DroneNotFoundException))]
        public void AddMaintenanceRecordInvalidDroneTest()
        {
            _factory.MaintenanceRecords.AddMaintenanceRecord(_firstMaintainerId, 0, MaintenanceRecordType.Maintenance, DateTime.Now, MaintenanceRecordDescription, MaintenanceRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(DroneNotFoundException))]
        public async Task AddMaintenanceRecordInvalidDroneAsyncTest()
        {
            await _factory.MaintenanceRecords.AddMaintenanceRecordAsync(_firstMaintainerId, 0, MaintenanceRecordType.Maintenance, DateTime.Now, MaintenanceRecordDescription, MaintenanceRecordNotes);
        }

        [TestMethod]
        public void GetMaintenanceRecordTest()
        {
            var maintenanceRecord = _factory.MaintenanceRecords.GetMaintenanceRecord(_firstMaintainerId);
            Assert.AreEqual(_firstMaintainerId, maintenanceRecord.MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecord.DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecord.RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecord.Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecord.Notes);
        }

        [TestMethod]
        public async Task GetMaintenanceRecordAsyncTest()
        {
            var maintenanceRecord = await _factory.MaintenanceRecords.GetMaintenanceRecordAsync(_firstMaintainerId);
            Assert.AreEqual(_firstMaintainerId, maintenanceRecord.MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecord.DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecord.RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecord.Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecord.Notes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintenanceRecordNotFoundException))]
        public void GetMaintenanceRecordInvalidTest()
        {
            _factory.MaintenanceRecords.GetMaintenanceRecord(0);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintenanceRecordNotFoundException))]
        public async Task GetMaintenanceRecordInvalidAsyncTest()
        {
            await _factory.MaintenanceRecords.GetMaintenanceRecordAsync(0);
        }

        [TestMethod]
        public void UpdateMaintenanceRecordTest()
        {
            _factory.MaintenanceRecords.UpdateMaintenanceRecord(_maintenanceRecordId, _secondMaintainerId, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
            _factory.Context.SaveChanges();

            Assert.AreEqual(1, _factory.Context.MaintenanceRecords.Count());
            Assert.AreEqual(_secondMaintainerId, _factory.Context.MaintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, _factory.Context.MaintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Modification, _factory.Context.MaintenanceRecords.First().RecordType);
            Assert.AreEqual(ModificationRecordDescription, _factory.Context.MaintenanceRecords.First().Description);
            Assert.AreEqual(ModificationRecordNotes, _factory.Context.MaintenanceRecords.First().Notes);
        }

        [TestMethod]
        public async Task UpdateMaintenanceRecordAsyncTest()
        {
            await _factory.MaintenanceRecords.UpdateMaintenanceRecordAsync(_maintenanceRecordId, _secondMaintainerId, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
            await _factory.Context.SaveChangesAsync();

            Assert.AreEqual(1, _factory.Context.MaintenanceRecords.Count());
            Assert.AreEqual(_secondMaintainerId, _factory.Context.MaintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, _factory.Context.MaintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Modification, _factory.Context.MaintenanceRecords.First().RecordType);
            Assert.AreEqual(ModificationRecordDescription, _factory.Context.MaintenanceRecords.First().Description);
            Assert.AreEqual(ModificationRecordNotes, _factory.Context.MaintenanceRecords.First().Notes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintenanceRecordNotFoundException))]
        public void UpdateMaintenanceRecordInvalidTest()
        {
            _factory.MaintenanceRecords.UpdateMaintenanceRecord(0, _secondMaintainerId, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintenanceRecordNotFoundException))]
        public async Task UpdateMaintenanceRecordInvalidAsyncTest()
        {
            await _factory.MaintenanceRecords.UpdateMaintenanceRecordAsync(0, _secondMaintainerId, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintainerNotFoundException))]
        public void UpdateMaintenanceRecordInvalidMaintainerTest()
        {
            _factory.MaintenanceRecords.UpdateMaintenanceRecord(_maintenanceRecordId, 0, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(MaintainerNotFoundException))]
        public async Task UpdateMaintenanceRecordInvalidMaintainerAsyncTest()
        {
            await _factory.MaintenanceRecords.UpdateMaintenanceRecordAsync(_maintenanceRecordId, 0, _droneId, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(DroneNotFoundException))]
        public void UpdateMaintenanceRecordInvalidDroneTest()
        {
            _factory.MaintenanceRecords.UpdateMaintenanceRecord(_maintenanceRecordId, _secondMaintainerId, 0, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
        }

        [TestMethod]
        [ExpectedException(typeof(DroneNotFoundException))]
        public async Task UpdateMaintenanceRecordInvalidDroneAsyncTest()
        {
            await _factory.MaintenanceRecords.UpdateMaintenanceRecordAsync(_maintenanceRecordId, _secondMaintainerId, 0, MaintenanceRecordType.Modification, DateTime.Now, ModificationRecordDescription, ModificationRecordNotes);
        }

        [TestMethod]
        public void FindMaintenanceRecordsByMaintainerTest()
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = _factory.MaintenanceRecords.FindMaintenanceRecords(_firstMaintainerId, null, null, null, 1, int.MaxValue);
            Assert.AreEqual(1, maintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, maintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecords.First().Notes);
        }

        [TestMethod]
        public async Task FindMaintenanceRecordsByMaintainerAsyncTest()
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = await _factory.MaintenanceRecords.FindMaintenanceRecordsAsync(_firstMaintainerId, null, null, null, 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, maintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, maintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecords.First().Notes);
        }

        [TestMethod]
        public void FindMaintenanceRecordsByDroneTest()
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = _factory.MaintenanceRecords.FindMaintenanceRecords(null, _droneId, null, null, 1, int.MaxValue);
            Assert.AreEqual(1, maintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, maintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecords.First().Notes);
        }

        [TestMethod]
        public async Task FindMaintenanceRecordsByDroneAsyncTest()
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = await _factory.MaintenanceRecords.FindMaintenanceRecordsAsync(null, _droneId, null, null, 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, maintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, maintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecords.First().Notes);
        }

        [TestMethod]
        public void FindMaintenanceRecordsByDateRangeTest()
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = _factory.MaintenanceRecords.FindMaintenanceRecords(null, null, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), 1, int.MaxValue);
            Assert.AreEqual(1, maintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, maintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecords.First().Notes);
        }

        [TestMethod]
        public async Task FindMaintenanceRecordsByDateRangeAsyncTest()
        {
            IEnumerable<MaintenanceRecord> maintenanceRecords = await _factory.MaintenanceRecords.FindMaintenanceRecordsAsync(null, null, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), 1, int.MaxValue).ToListAsync();
            Assert.AreEqual(1, maintenanceRecords.Count());
            Assert.AreEqual(_firstMaintainerId, maintenanceRecords.First().MaintainerId);
            Assert.AreEqual(_droneId, maintenanceRecords.First().DroneId);
            Assert.AreEqual(MaintenanceRecordType.Maintenance, maintenanceRecords.First().RecordType);
            Assert.AreEqual(MaintenanceRecordDescription, maintenanceRecords.First().Description);
            Assert.AreEqual(MaintenanceRecordNotes, maintenanceRecords.First().Notes);
        }
    }
}
