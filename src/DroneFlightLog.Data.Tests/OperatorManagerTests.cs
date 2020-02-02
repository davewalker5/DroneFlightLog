using System;
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
    public class OperatorManagerTests
    {
        private const string Number = "1";
        private const string Street = "Some Street";
        private const string Town = "Some Town";
        private const string County = "Some County";
        private const string Postcode = "AB12 3CD";
        private const string SecondPostcode = "EF12 3GH";
        private const string Country = "Some Country";

        private const string FirstNames = "First Names";
        private const string Surname = "Surname";
        private readonly DateTime DoB = DateTime.Now;
        private const string FlyerNumber = "Some Flyer Number";
        private const string OperatorNumber = "Some Operator Number";

        private IDroneFlightLogFactory<DroneFlightLogDbContext> _factory;
        private int _firstAddressId;
        private int _secondAddressId;
        private int _operatorId;

        [TestInitialize]
        public void TestInitialize()
        {
            DroneFlightLogDbContext context = new DroneFlightLogDbContextFactory().CreateDbContext(null);
            _factory = new DroneFlightLogFactory<DroneFlightLogDbContext>(context);

            Address firstAddress = _factory.Addresses.AddAddress(Number, Street, Town, County, Postcode, Country);
            Address secondAddress = _factory.Addresses.AddAddress(Number, Street, Town, County, SecondPostcode, Country);
            _factory.Context.SaveChanges();
            _firstAddressId = firstAddress.Id;
            _secondAddressId = secondAddress.Id;

            Operator op = _factory.Operators.AddOperator(FirstNames, Surname, DoB, FlyerNumber, OperatorNumber, _firstAddressId);
            _factory.Context.SaveChanges();
            _operatorId = op.Id;
        }

        [TestMethod]
        public void AddOperatorTest()
        {
            // The operator has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Operators.Count());
            Assert.AreEqual(FirstNames, _factory.Context.Operators.First().FirstNames);
            Assert.AreEqual(Surname, _factory.Context.Operators.First().Surname);
            Assert.AreEqual(DoB, _factory.Context.Operators.First().DoB);
            Assert.AreEqual(FlyerNumber, _factory.Context.Operators.First().FlyerNumber);
            Assert.AreEqual(OperatorNumber, _factory.Context.Operators.First().OperatorNumber);
            Assert.AreEqual(_firstAddressId, _factory.Context.Operators.First().AddressId);
        }

        [TestMethod, ExpectedException(typeof(OperatorExistsException))]
        public void AddExistingOperatorTest()
        {
            _factory.Operators.AddOperator(FirstNames, Surname, DateTime.Now, "", "", _firstAddressId);
        }

        [TestMethod]
        public void GetOperatorByIdTest()
        {
            Operator op = _factory.Operators.GetOperator(_operatorId);
            ValidateOperator(op, _operatorId, _firstAddressId);
        }

        [TestMethod, ExpectedException(typeof(OperatorNotFoundException))]
        public void GetMissingOperatorByIdTest()
        {
            _factory.Operators.GetOperator(-1);
        }

        [TestMethod]
        public void GetAllOperatorsTest()
        {
            IEnumerable<Operator> operators = _factory.Operators.GetOperators(null);
            Assert.AreEqual(1, operators.Count());
            ValidateOperator(operators.First(), _operatorId, _firstAddressId);
        }

        [TestMethod]
        public void GetAllOperatorsForAddressTest()
        {
            IEnumerable<Operator> operators = _factory.Operators.GetOperators(_firstAddressId);
            Assert.AreEqual(1, operators.Count());
            ValidateOperator(operators.First(), _operatorId, _firstAddressId);
        }

        [TestMethod]
        public void GetAllOperatorsForMissingAddressTest()
        {
            IEnumerable<Operator> operators = _factory.Operators.GetOperators(-1);
            Assert.AreEqual(0, operators.Count());
        }

        [TestMethod]
        public void FindOperatorTest()
        {
            Operator op = _factory.Operators.FindOperator(FirstNames, Surname, _firstAddressId);
            ValidateOperator(op, _operatorId, _firstAddressId);
        }

        [TestMethod]
        public void FindMissingOperatorTest()
        {
            Operator op = _factory.Operators.FindOperator("", "", 0);
            Assert.IsNull(op);
        }

        [TestMethod]
        public void SetOperatorAddressTest()
        {
            _factory.Operators.SetOperatorAddress(_operatorId, _secondAddressId);
            _factory.Context.SaveChanges();

            Operator op = _factory.Operators.FindOperator(FirstNames, Surname, _firstAddressId);
            Assert.IsNull(op);

            op = _factory.Operators.FindOperator(FirstNames, Surname, _secondAddressId);
            ValidateOperator(op, _operatorId, _secondAddressId);
        }

        [TestMethod, ExpectedException(typeof(AddressNotFoundException))]
        public void SetOperatorMissingAddressTest()
        {
            _factory.Operators.SetOperatorAddress(_operatorId, -1);
        }

        [TestMethod, ExpectedException(typeof(OperatorNotFoundException))]
        public void SetMissingOperatorAddressTest()
        {
            _factory.Operators.SetOperatorAddress(-1, _secondAddressId);
        }

        #region Helpers
        /// <summary>
        /// Validate the specified operator
        /// </summary>
        /// <param name="op"></param>
        /// <param name="expectedOperatorId"></param>
        /// <param name="expectedAddressId"></param>
        private void ValidateOperator(Operator op, int expectedOperatorId, int expectedAddressId)
        {
            Assert.AreEqual(expectedOperatorId, op.Id);

            Assert.AreEqual(FirstNames, op.FirstNames);
            Assert.AreEqual(Surname, op.Surname);
            Assert.AreEqual(DoB, op.DoB);
            Assert.AreEqual(FlyerNumber, op.FlyerNumber);
            Assert.AreEqual(OperatorNumber, op.OperatorNumber);
            Assert.AreEqual(expectedAddressId, op.AddressId);
        }
        #endregion
    }
}
