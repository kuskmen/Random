using Microsoft.AspNetCore.Mvc;
using Shameful_MVC.Data;
using Shameful_MVC.Views.Home;

namespace Shameful_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly shameful_mvcContext _context;

        public LoginController(shameful_mvcContext context)
        {
            _context = context;
        }

        public IActionResult Index(LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Login([Bind("FacultyNumber,Password")] LoginViewModel model)
        {
            if (ModelState.IsValid && _context.Students.Find(model.FacultyNumber) != null)
            {
                return RedirectToAction("Index", "Home");
            }

            model.SuccessfulLogin = false;
            return View("Index", model);
        }

        public IActionResult Register()
        {
            return RedirectToAction("Index", "Registration");
        }
    }
}
