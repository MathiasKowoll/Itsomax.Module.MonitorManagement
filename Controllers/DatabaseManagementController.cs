﻿using Itsomax.Module.Core.Models;
using Itsomax.Module.MonitorCore.Interfaces;
using Itsomax.Module.MonitorCore.ViewModels.DatabaseManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NToastNotify;
using Itsomax.Module.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
            ViewBag.VendorList = _monitor.VendorSelectList(-1);
            ViewBag.ConfigurationTypeList = _monitor.ConfigurationTypeSelectList(-1);
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
            ViewBag.VendorList = _monitor.VendorSelectList(-1);
            ViewBag.ConfigurationTypeList = _monitor.ConfigurationTypeSelectList(-1);
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
            ViewBag.VendorList = _monitor.VendorSelectList(model.VendorId);
            ViewBag.ConfigurationTypeList = _monitor.ConfigurationTypeSelectList(model.ConfigTypeId);
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
            _logger.InformationLog("System" + model.Name + " deleted succesfully", "Delete System", string.Empty, GetCurrentUserAsync().Result.UserName);
            return Json(true);

        }
        [HttpDelete]
        [Route("/state/system/{id}")]
        public IActionResult StateSystemView(long id)
        {
            var model = _monitor.GetDatabaseSystemById(id,GetCurrentUserAsync().Result.UserName);
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
        #endregion

        #region Service

        public IActionResult ServiceList()
        {
            return View();
        }

        [Route("/get/service/json")]
        public JsonResult ServiceListJson()
        {
            return Json(_monitor.GetServicesList(GetCurrentUserAsync().Result.UserName));
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