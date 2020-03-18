using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class OperatorsController : Controller
    {
        private readonly OperatorClient _operators;
        private readonly AddressClient _addresses;

        public OperatorsController(OperatorClient operators, AddressClient addresses)
        {
            _operators = operators;
            _addresses = addresses;
        }

        /// <summary>
        /// Serve the operators list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Operator> operators = await _operators.GetOperatorsAsync();
            return View(operators);
        }

        /// <summary>
        /// Serve the page to add a new operator
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddOperatorViewModel());
        }

        /// <summary>
        /// Handle POST events to save new operators
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddOperatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                Address address = await _addresses.FindOrAddAddressAsync(
                                                model.Address.Number,
                                                model.Address.Street,
                                                model.Address.Town,
                                                model.Address.County,
                                                model.Address.Postcode,
                                                model.Address.Country);

                DateTime doB = DateTime.Parse(model.OperatorDateOfBirth);
                Operator op = await _operators.AddOperatorAsync(
                                                model.Operator.FirstNames,
                                                model.Operator.Surname,
                                                model.Operator.OperatorNumber,
                                                model.Operator.FlyerNumber,
                                                doB,
                                                address.Id);

                ModelState.Clear();
                model.Clear();
                model.Message = $"Operator '{op.FirstNames} {op.Surname}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing operator
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Operator op = await _operators.GetOperatorAsync(id);
            EditOperatorViewModel model = new EditOperatorViewModel();
            model.Operator = op;
            model.Address = op.Address;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update existing operators
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditOperatorViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                Address address = await _addresses.FindOrAddAddressAsync(
                                                model.Address.Number,
                                                model.Address.Street,
                                                model.Address.Town,
                                                model.Address.County,
                                                model.Address.Postcode,
                                                model.Address.Country);

                DateTime doB = DateTime.Parse(model.OperatorDateOfBirth);
                Operator op = await _operators.UpdateOperatorAsync(
                                                model.Operator.Id,
                                                model.Operator.FirstNames,
                                                model.Operator.Surname,
                                                model.Operator.OperatorNumber,
                                                model.Operator.FlyerNumber,
                                                doB,
                                                address.Id);

                result = RedirectToAction("Index");
            }
            else
            {
                result = View(model);
            }

            return result;
        }
    }
}
