using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shameful_MVC.Data;
using Shameful_MVC.Utilities;
using Shameful_MVC.Views.Home;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shameful_MVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly shameful_mvcContext _context;

        public LoginController(shameful_mvcContext context)
        {
            _context = context;
        }

        public IActionResult Index(LoginViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(AssignmentsController.Index), "Assignments");
            }

            if (model == null)
            {
                model = new LoginViewModel();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("FacultyNumber,Password")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _context.Students.Find(model.FacultyNumber);
                if (student != null && student.Password.Equals(model.Password))
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, AuthenticateUser(model.FacultyNumber));

                    return RedirectToAction(nameof(AssignmentsController.Index), "Assignments");
                }
            }

            model.SuccessfulLogin = false;
            return View(nameof(LoginController.Index), model);
        }

        public static ClaimsPrincipal AuthenticateUser(string facultyNumber)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(PolicyConstants.FacultyNumberPolicyClaim, facultyNumber));

            return new ClaimsPrincipal(identity);
        }
    }
}
