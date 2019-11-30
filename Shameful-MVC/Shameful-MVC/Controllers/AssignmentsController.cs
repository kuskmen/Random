using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shameful_MVC.Data;
using Shameful_MVC.Models;
using Shameful_MVC.Utilities;
using Shameful_MVC.Views.Assignments;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shameful_MVC.Controllers
{
    [Authorize(Policy = PolicyConstants.LoggedInPolicy)]
    public class AssignmentsController : Controller
    {
        private readonly shameful_mvcContext _context;

        public AssignmentsController(shameful_mvcContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Assignments.AsEnumerable());
        }

        public IActionResult AssignmentsAddForm()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            return File(
                _context.Assignments.FirstOrDefault(assignment => assignment.Id == id).File,
                "application/x-zip-compressed");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AssignmentViewModel assignmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var ms = new MemoryStream();
                assignmentViewModel.FormFile.CopyTo(ms);

                var assignment = new Assignment()
                {
                    Date = DateTime.UtcNow,
                    Name = assignmentViewModel.Name,
                    File = ms.ToArray(),
                };

                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Assignments");
            }

            return View(nameof(AssignmentsAddForm));
        }
    }
}
