using Microsoft.AspNetCore.Mvc;

namespace Stock_Management_System.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("~/[controller]/[action]")]
    public class RemainderController : Controller
    {
        public IActionResult Create_Remainder()
        {
            return View();
        }
    }
}
