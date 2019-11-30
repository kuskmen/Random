using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shameful_MVC.Data;
using Shameful_MVC.Models;
using Shameful_MVC.Views.Registration;
using System.Threading.Tasks;

namespace Shameful_MVC.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : Controller
    {
        private readonly shameful_mvcContext _context;

        public RegistrationController(shameful_mvcContext context)
        {
            _context = context;
        }

        public IActionResult Index(RegistrationViewModel model)
        {
            if (model != null)
            {
                return View("Index", model);
            }

            var registrationModel = new RegistrationViewModel();
            return View(registrationModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,MiddleName,LastName,FacultyNumber,Password,ConfirmPassword,Specialty,Year,FormOfEducation")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View("Index", new RegistrationViewModel
            {
                Student = student,
            });
        }
    }
}
