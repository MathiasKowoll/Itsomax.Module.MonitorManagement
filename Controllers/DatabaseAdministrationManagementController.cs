using Microsoft.AspNetCore.Mvc;

namespace Itsomax.Module.MonitorManagement.Controllers
{
    public class DatabaseAdministrationManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}