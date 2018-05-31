using Itsomax.Module.Core.Models;
using Itsomax.Module.MonitorCore.Interfaces;
using Itsomax.Module.MonitorCore.ViewModels.DatabaseManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NToastNotify;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.MonitorCore.Data;

namespace Itsomax.Module.MonitorManagement.Controllers
{
    public class DatabaseManagementController : Controller
    {
        private readonly IMonitor _monitor;
        private readonly UserManager<User> _userManager;
        private readonly IDatabaseSystemRepository _databaseSystem;
        private readonly IToastNotification _toastNotification;
        private readonly ILogginToDatabase _logger;

        public DatabaseManagementController(IMonitor monitor, UserManager<User> userManager,
            IToastNotification toastNotification,
            ILogginToDatabase logger, IDatabaseSystemRepository databaseSystem)
        {
            _monitor = monitor;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _logger = logger;
            _databaseSystem = databaseSystem;
        }


        public IActionResult SystemList()
        {
            return View();
        }

        [HttpGet]
        [Route("/get/all/systems/json")]
        public JsonResult SystemListJson()
        {
            return Json(_monitor.GetSystemList(GetCurrentUserAsync().Result.UserName));
        }

        public IActionResult CreateSystem()
        {
            ViewBag.VendorList = _monitor.VendorSelectList(-1);
            ViewBag.ConfigurationTypeList = _monitor.ConfigurationTypeSelectList(-1);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSystemPostView(CreateSystemViewModel model)
        {
            if (!ModelState.IsValid) return View(nameof(SystemList), model);
            var result = _monitor.CreateSystem(model, GetCurrentUserAsync().Result.UserName).Result;
            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(result.OkMessage, new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                ViewBag.VendorList = _monitor.VendorSelectList(-1);
                ViewBag.ConfigurationTypeList = _monitor.ConfigurationTypeSelectList(-1);
                return View(nameof(SystemList));
            }

            _toastNotification.AddErrorToastMessage(result.Errors, new ToastrOptions()
            {
                PositionClass = ToastPositions.TopCenter
            });
            ViewBag.VendorList = _monitor.VendorSelectList(-1);
            ViewBag.ConfigurationTypeList = _monitor.ConfigurationTypeSelectList(-1);
            return View(nameof(CreateSystem), model);
        }

        [HttpGet]
        [Route("/get/system/{id}")]
        public IActionResult EditSystemView(long id)
        {
            var model = _monitor.GetSystemForEdit(id, GetCurrentUserAsync().Result.UserName);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSystemViewPost(EditSystemViewModel model)
        {
            if (!ModelState.IsValid) return View(nameof(SystemList));
            var res = await _monitor.EditSystem(model, GetCurrentUserAsync().Result.UserName);
            if (res.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(res.OkMessage, new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return View(nameof(SystemList));
            }
            _toastNotification.AddErrorToastMessage(res.Errors, new ToastrOptions()
            {
                PositionClass = ToastPositions.TopCenter
            });
            return View(nameof(EditSystemView), model);
        }

        [HttpDelete]
        [Route("/delete/system/{id}")]
        public IActionResult DeleteSystemView(long id)
        {
            var model = _databaseSystem.GetById(id);
            if(model == null)
            {
                return Json(null);
            }

            if (!_monitor.DeleteSystem(id, GetCurrentUserAsync().Result.UserName)) return Json(false);
            _logger.InformationLog("System" + model.Name + " deleted succesfully", "Delete System", string.Empty, GetCurrentUserAsync().Result.UserName);
            return Json(true);

        }
        [HttpDelete]
        [Route("/state/system/{id}")]
        public IActionResult StateSystemView(long id)
        {
            var model = _databaseSystem.GetById(id);
            if (model == null)
            {
                return Json(null);
            }
            if (_monitor.DisableEnableSystem(id, GetCurrentUserAsync().Result.UserName))
            {

                _logger.InformationLog("System" + model.Name + " deleted succesfully", "Delete System", string.Empty, GetCurrentUserAsync().Result.UserName);
                return Json(true);
            }
            else
            {
                return Json(false);
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