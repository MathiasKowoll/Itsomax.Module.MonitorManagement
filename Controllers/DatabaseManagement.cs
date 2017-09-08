using Itsomax.Module.Core.Models;
using Itsomax.Module.MonitorManagement.Interfaces;
using Itsomax.Module.MonitorManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NToastNotify;
using Itsomax.Module.Core.Interfaces;

namespace Itsomax.Module.MonitorManagement.Controllers
{
    public class DatabaseManagement : Controller
    {
        public IMonitor _monitor;
        private readonly UserManager<User> _userManager;
        private IToastNotification _toastNotification;
        private readonly ILogginToDatabase _logger;

        public DatabaseManagement(IMonitor monitor, UserManager<User> userManager, IToastNotification toastNotification,
            ILogginToDatabase logger)
        {
            _monitor = monitor;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _logger = logger;
        }


        public IActionResult SystemList()
        {
            return View();
        }

        [HttpGet]
        [Route("/get/all/systems/json")]
        public JsonResult ListSystem()
        {
            return Json(_monitor.GetSystemList(GetCurrentUserAsync().Result.UserName));
        }

        [HttpGet]
        public IActionResult EditSystemView(long Id)
        {
            var model = _monitor.GetSystemForEdit(Id, GetCurrentUserAsync().Result.UserName);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSystemView(EditSystemViewModel model)
        {
            return RedirectToAction(nameof(SystemList));
        }


        public IActionResult CreateSystem()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSystemPostView(CreateSystemViewModel model)
        {
            var result = _monitor.CreateSystem(model, GetCurrentUserAsync().Result.UserName);
            if(result)
            {
                _toastNotification.AddToastMessage("System: " + model.Name + " created succesfully", "", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                _logger.InformationLog("System" + model.Name + " created succesfully", "Create System", string.Empty, GetCurrentUserAsync().Result.UserName);
                return View();
            }
            else
            {
                return View(nameof(CreateSystem),model);
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string AddErrorList(IdentityResult result)
        {
            var errorList = string.Empty;
            foreach (var error in result.Errors)
            {
                errorList = errorList + " " + error.Description;
            }
            return errorList;
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        #endregion
    }
}