using DroneFlightLog.Mvc.Api;
using DroneFlightLog.Mvc.Entities;
using DroneFlightLog.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class MaintainersController : Controller
    {
        private readonly MaintainersClient _operators;

        public MaintainersController(MaintainersClient maintainers)
        {
            _operators = maintainers;
        }

        /// <summary>
        /// Serve the maintainers list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Maintainer> operators = await _operators.GetMaintainersAsync();
            return View(operators);
        }

        /// <summary>
        /// Serve the page to add a new maintainer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddMaintainerViewModel());
        }

        /// <summary>
        /// Handle POST events to save new maintainers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMaintainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Maintainer maintainer = await _operators.AddMaintainerAsync(
                                                model.Maintainer.FirstNames,
                                                model.Maintainer.Surname);

                ModelState.Clear();
                model.Clear();
                model.Message = $"Maintainer '{maintainer.FirstNames} {maintainer.Surname}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to edit an existing maintainer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Maintainer maintainer = await _operators.GetMaintainerAsync(id);
            EditMaintainerViewModel model = new EditMaintainerViewModel();
            model.Maintainer = maintainer;
            return View(model);
        }

        /// <summary>
        /// Handle POST events to update existing maintainers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMaintainerViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                Maintainer maintainer = await _operators.UpdateMaintainerAsync(
                                                model.Maintainer.Id,
                                                model.Maintainer.FirstNames,
                                                model.Maintainer.Surname);

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
