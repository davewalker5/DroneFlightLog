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
    public class ModelManagerTest
    {
        private const string ManufacturerName = "Some Manufacturer";
        private const string ModelName = "Some Model";
        private const string AsyncModelName = "Some Async Model";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _manufacturerId;
        private int _modelId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Manufacturer manufacturer = _factory.Manufacturers.AddManufacturer(ManufacturerName);
            _factory.Context.SaveChanges();
            _manufacturerId = manufacturer.Id;

            Model model = _factory.Models.AddModel(ModelName, _manufacturerId);
            _factory.Context.SaveChanges();
            _modelId = model.Id;
        }

        [TestMethod]
        public void AddModelTest()
        {
            // The  model has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Models.Count());
            Assert.AreEqual(ModelName, _factory.Context.Models.First().Name);
            Assert.AreEqual(_manufacturerId, _factory.Context.Models.First().ManufacturerId);
        }

        [TestMethod]
        public async Task AddModelAsyncTest()
        {
            Model model = await _factory.Models.AddModelAsync(AsyncModelName, _manufacturerId);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Models.Count());
            Assert.AreEqual(AsyncModelName, model.Name);
            Assert.AreEqual(_manufacturerId, model.ManufacturerId);
        }

        [TestMethod, ExpectedException(typeof(ModelExistsException))]
        public void AddExistingModelTest()
        {
            _factory.Models.AddModel(ModelName, _manufacturerId);
        }

        [TestMethod, ExpectedException(typeof(ManufacturerNotFoundException))]
        public void AddModelForMissingManufacturerTest()
        {
            _factory.Models.AddModel(ModelName, -1);
        }

        [TestMethod]
        public void GetModelByIdTest()
        {
            Model model = _factory.Models.GetModel(_modelId);
            ValidateModel(model);
        }

        [TestMethod]
        public async Task GetModelByIdAsyncTest()
        {
            Model model = await _factory.Models.GetModelAsync(_modelId);
            ValidateModel(model);
        }

        [TestMethod, ExpectedException(typeof(ModelNotFoundException))]
        public void GetMissingModelByIdTest()
        {
            _factory.Models.GetModel(-1);
        }

        [TestMethod]
        public void GetAllModelsTest()
        {
            IEnumerable<Model> models = _factory.Models.GetModels(null);
            Assert.AreEqual(1, models.Count());
            ValidateModel(models.First());
        }

        [TestMethod]
        public async Task GetAllModelsAsyncTest()
        {
            List<Model> models = await _factory.Models.GetModelsAsync(null).ToListAsync();
            Assert.AreEqual(1, models.Count());
            ValidateModel(models.First());
        }

        [TestMethod]
        public void GetModelsForManufacturerTest()
        {
            IEnumerable<Model> models = _factory.Models.GetModels(_manufacturerId);
            Assert.AreEqual(1, models.Count());
            ValidateModel(models.First());
        }

        [TestMethod]
        public void GetModelsForMissingManufacturerTest()
        {
            IEnumerable<Model> models = _factory.Models.GetModels(-1);
            Assert.IsFalse(models.Any());
        }

        #region Helpers
        /// <summary>
        /// Check the properties of a model
        /// </summary>
        /// <param name="model"></param>
        private void ValidateModel(Model model)
        {
            Assert.AreEqual(_modelId, model.Id);
            Assert.AreEqual(ModelName, model.Name);
            Assert.AreEqual(_manufacturerId, model.ManufacturerId);
            Assert.AreEqual(ManufacturerName, model.Manufacturer.Name);
        }
        #endregion
    }
}
