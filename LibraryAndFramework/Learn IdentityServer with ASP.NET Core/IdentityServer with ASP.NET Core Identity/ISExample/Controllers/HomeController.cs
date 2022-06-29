using Microsoft.AspNetCore.Mvc;

namespace ISExample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
