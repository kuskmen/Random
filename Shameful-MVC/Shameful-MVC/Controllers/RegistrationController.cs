using Microsoft.AspNetCore.Mvc;
using Shameful_MVC.Models;
using System.Threading.Tasks;

namespace Shameful_MVC.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly shameful_mvcContext _context;

        public RegistrationController(shameful_mvcContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,MiddleName,LastName,FacultyNumber,Password,ConfirmPassword,Specialty,Year,FormOfEducation")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }
    }
}
