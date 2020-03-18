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
            return View(new OperatorViewModel());
        }

        /// <summary>
        /// Handle POST events to save new locations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(OperatorViewModel model)
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
    }
}
