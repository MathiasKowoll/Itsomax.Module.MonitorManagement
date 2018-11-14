using System.Linq;
using Itsomax.Module.Core.Models;
using Itsomax.Module.MonitorCore.Interfaces;
using Itsomax.Module.MonitorCore.ViewModels.DatabaseManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NToastNotify;
using Itsomax.Module.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Itsomax.Module.MonitorManagement.Controllers
{
    [Authorize(Policy = "ManageAuthentification")]
    public class DatabaseManagementController : Controller
    {
        private readonly IMonitor _monitor;
        private readonly UserManager<User> _userManager;
        private readonly IToastNotification _toastNotification;
        private readonly ILogginToDatabase _logger;

        public DatabaseManagementController(IMonitor monitor, UserManager<User> userManager,
            IToastNotification toastNotification,ILogginToDatabase logger)
        {
            _monitor = monitor;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _logger = logger;

        }

        #region Database System
        
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
            
            ViewBag.VendorList = from v in  _monitor.VendorSelectList(-1)
                select new {VendorId = v.Id, VendorName = v.Name};
            ViewBag.EnvironmentList = _monitor.EnvironmentSelectList(-1);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSystem(CreateSystemViewModel model)
        {
            if (!ModelState.IsValid) return View(nameof(SystemList), model);
            var result = _monitor.CreateSystem(model, GetCurrentUserAsync().Result.UserName).Result;
            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(result.OkMessage, new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction("SystemList");
            }

            _toastNotification.AddErrorToastMessage(result.Errors, new ToastrOptions()
            {
                PositionClass = ToastPositions.TopCenter
            });
            ViewBag.VendorList = from v in  _monitor.VendorSelectList(-1)
                select new {VendorId = v.Id, VendorName = v.Name};
            ViewBag.EnvironmentList = _monitor.EnvironmentSelectList(-1);
            return View(nameof(CreateSystem), model);
        }

        [HttpGet]
        [Route("/get/system/{id}")]
        public IActionResult EditSystemView(long id)
        {
            var model = _monitor.GetSystemForEdit(id,GetCurrentUserAsync().Result.UserName);
            if (model == null)
            {
                _toastNotification.AddErrorToastMessage("System does not exist", new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction("SystemList");
            }
            
            var vendorList = _monitor.VendorSelectList(model.VendorId);
            if (vendorList == null)
            {
                _toastNotification.AddErrorToastMessage("Vendor does not exist", new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction("SystemList");
            }
            var configurationTypeList = _monitor.ConfigurationTypeSelectList(model.ConfigTypeId);
            if (configurationTypeList == null)
            {
                _toastNotification.AddErrorToastMessage("Configuration type does not exit", new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                    
                return RedirectToAction("SystemList");
            }
            //ViewBag.VendorList = _monitor.VendorSelectList(model.VendorId);
            ViewBag.ConfigList = from c in _monitor.GetConfigurationByVendor(model.VendorId)
                select new {ConfigId = c.Id, ConfigName = c.Name};
            ViewBag.VendorList = from v in  _monitor.VendorSelectList(model.VendorId)
                select new {VendorId = v.Id, VendorName = v.Name};
            ViewBag.EnvironmentList = _monitor.EnvironmentSelectList(model.EnvironmentId);
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSystemViewPost(EditSystemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/get/system/"+model.Id.ToString());
            }
            var res = await _monitor.EditSystem(model, GetCurrentUserAsync().Result.UserName);
            if (res.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(res.OkMessage, new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction("SystemList");
            }
            _toastNotification.AddErrorToastMessage(res.Errors, new ToastrOptions()
            {
                PositionClass = ToastPositions.TopCenter
            });
            return RedirectToPage("/get/system/"+model.Id.ToString());
        }
        
        [HttpDelete]
        [Route("/delete/system/{id}")]
        public IActionResult DeleteSystemView(long id)
        {
            var model = _monitor.GetDatabaseSystemById(id,GetCurrentUserAsync().Result.UserName);
            if(model == null)
            {
                return Json(null);
            }

            if (!_monitor.DeleteSystem(id, GetCurrentUserAsync().Result.UserName)) return Json(false);
            _logger.InformationLog("System" + model.Name + " deleted succesfully", "Delete System", string.Empty,
                GetCurrentUserAsync().Result.UserName);
            return Json(true);

        }
        [HttpDelete]
        [Route("/state/system/{id}")]
        public async Task<JsonResult> StateSystemView(long id)
        {
            var model = _monitor.GetDatabaseSystemById(id,GetCurrentUserAsync().Result.UserName);
            if (model == null)
            {
                return Json(false);
            }
            if (await _monitor.DisableEnableSystem(id, GetCurrentUserAsync().Result.UserName))
            {
                return Json(true);
            }

            return Json(false);
        }
        #endregion

        #region Service
        
        [Route("/get/services/by/system/")]
        [Route("/get/services/by/system/{id}")]
        public IActionResult ServiceList(long? id)
        {
            var system= _monitor.GetDatabaseSystemById(id ?? 0,GetCurrentUserAsync().Result.UserName);
            ViewBag.SystemName = system != null ? system.Name : "All Systems";
            
            ViewBag.SystemId = id.ToString() ?? "";    
            return View();
        }
        
        [Route("/get/services/json/")]
        [Route("/get/service/json/{id}")]
        public JsonResult ServiceListJson(long? id)
        {
            return Json(_monitor.GetServicesList(id,GetCurrentUserAsync().Result.UserName));
        }
        
        public IActionResult CreateService()
        {
            ViewBag.DataBaseList = _monitor.DatabaseSystemList(-1);
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(CreateServiceViewModel model)
        {
            var result = await _monitor.CreateService(model, GetCurrentUserAsync().Result.UserName);
            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(result.OkMessage, new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction("SystemList");
            }

            _toastNotification.AddErrorToastMessage(result.Errors, new ToastrOptions()
            {
                PositionClass = ToastPositions.TopCenter
            });
            ViewBag.DataBaseList = _monitor.DatabaseSystemList(-1);
            return View(model);

        }
        
        public JsonResult GetConfigurationById(long vendorId)
        {
            var list = from c in _monitor.GetConfigurationByVendor(vendorId)
                select new
                {
                    ConfigId = c.Id,
                    ConfigName = c.Name
                };
            return Json(new SelectList(list,"ConfigId","ConfigName"));
        }

        [Route("/get/service/{id}")]
        public IActionResult EditService(long id)
        {
            
            var serviceToEdit = _monitor.GetServiceToEdit(id,GetCurrentUserAsync().Result.UserName);
            if (serviceToEdit == null)
            {
                _toastNotification.AddSuccessToastMessage("Service Not found", new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction("ServiceList");
            }

            ViewBag.Pass = serviceToEdit.LoginPassword;
            ViewBag.DataBaseList = _monitor.DatabaseSystemList(serviceToEdit.DatabaseSystemId);
            return View(serviceToEdit);
        }
        
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> EditServicePost(EditServiceViewModel model)
        {
            var result = await _monitor.EditService(model, GetCurrentUserAsync().Result.UserName);
            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage(result.OkMessage, new ToastrOptions()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction("ServiceList");
            }

            _toastNotification.AddErrorToastMessage(result.Errors, new ToastrOptions()
            {
                PositionClass = ToastPositions.TopCenter
            });
            ViewBag.DataBaseList = _monitor.DatabaseSystemList(model.DatabaseSystemId);
            return RedirectToAction(nameof(EditService), model.Id);
        }
        

        #endregion


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