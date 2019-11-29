using Microsoft.AspNetCore.Mvc;
using Shameful_MVC.Data;
using Shameful_MVC.Models;
using Shameful_MVC.Utilities;
using Shameful_MVC.Views.Assignments;
using System;
using System.Threading.Tasks;

namespace Shameful_MVC.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly shameful_mvcContext _context;

        public AssignmentsController(shameful_mvcContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssignmentsAddForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AssignmentViewModel assignmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var assignment = new Assignment()
                {
                    Date = DateTime.UtcNow,
                    Name = assignmentViewModel.Name,
                    File = await FileHelpers.ProcessFormFile(assignmentViewModel.FormFile, ModelState),
                };

                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(AssignmentsAddForm));
        }
    }
}
