using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itsomax.Module.MonitorManagement.Controllers
{
    class DatabaseManagement : Controller
    {
        public IActionResult DashBoard()
        {
            return View();
        }
    }
}
