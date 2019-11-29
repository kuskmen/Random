using Microsoft.AspNetCore.Mvc;

namespace Shameful_MVC.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}