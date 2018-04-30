using Itsomax.Module.Core.Models;
using Itsomax.Module.MonitorCore.Interfaces;
using Itsomax.Module.MonitorCore.ViewModels.DatabaseManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NToastNotify;
using Itsomax.Module.Core.Interfaces;

namespace Itsomax.Module.MonitorManagement.Controllers
{
    public class DatabaseManagementController : Controller
    {
        public IMonitor _monitor;
        private readonly UserManager<User> _userManager;
        private IToastNotification _toastNotification;
        private readonly ILogginToDatabase _logger;

        public DatabaseManagementController(IMonitor monitor, UserManager<User> userManager, IToastNotification toastNotification,
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
        public JsonResult SystemListJson()
        {
            return Json(_monitor.GetSystemList(GetCurrentUserAsync().Result.UserName));
        }

        public IActionResult CreateSystem()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSystemPostView(CreateSystemViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = _monitor.CreateSystem(model, GetCurrentUserAsync().Result.UserName);
                if (result)
                {
					_toastNotification.AddSuccessToastMessage("System: " + model.Name + " created succesfully", new ToastrOptions()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                    _logger.InformationLog("System" + model.Name + " created succesfully", "Create System", string.Empty, GetCurrentUserAsync().Result.UserName);
                    return View(nameof(SystemList));
                }
                else
                {
                    return View(nameof(CreateSystem), model);
                }
            }
            return View(nameof(CreateSystem), model);
        }

        [HttpGet]
        [Route("/get/system/{Id}")]
        public IActionResult EditSystemView(long Id)
        {
            var model = _monitor.GetSystemForEdit(Id, GetCurrentUserAsync().Result.UserName);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSystemViewPost(EditSystemViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_monitor.EditSystem(model, GetCurrentUserAsync().Result.UserName))
                {
					_toastNotification.AddSuccessToastMessage("System: " + model.Name + " edited succesfully", new ToastrOptions()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                    _logger.InformationLog("System" + model.Name + " edited succesfully", "Edit System", string.Empty, GetCurrentUserAsync().Result.UserName);
                    return View(nameof(SystemList));
                }
                else
                {
                    return View(nameof(EditSystemView), model);
                }

            }
            return View(nameof(SystemList));
        }

        [HttpDelete]
        [Route("/delete/system/{Id}")]
        public IActionResult DeleteSystemView(long Id)
        {
            var model = _monitor.GetSystem(Id);
            if(model == null)
            {
                return Json(null);
            }
            if (_monitor.DeleteSystem(Id, GetCurrentUserAsync().Result.UserName))
            {   
                _logger.InformationLog("System" + model.Name + " deleted succesfully", "Delete System", string.Empty, GetCurrentUserAsync().Result.UserName);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        [HttpDelete]
        [Route("/state/system/{Id}")]
        public IActionResult StateSystemView(long Id)
        {
            var model = _monitor.GetSystem(Id);
            if (model == null)
            {
                return Json(null);
            }
            if (_monitor.DisableEnableSystem(Id, GetCurrentUserAsync().Result.UserName))
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