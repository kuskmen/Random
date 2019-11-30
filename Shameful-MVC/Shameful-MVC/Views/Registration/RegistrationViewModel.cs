using Microsoft.AspNetCore.Mvc.Rendering;
using Shameful_MVC.Models;
using System.Collections.Generic;

namespace Shameful_MVC.Views.Registration
{
    public class RegistrationViewModel
    {
        public Student Student { get; set; }

        public IList<SelectListItem> FormOfEducations { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "full-time", Value = "1" },
            new SelectListItem { Text = "part-time", Value = "2" },
        };

        public IList<SelectListItem> Years { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "1 year", Value = "1" },
            new SelectListItem { Text = "2 year", Value = "2" },
            new SelectListItem { Text = "3 year", Value = "3" },
            new SelectListItem { Text = "4 year", Value = "4" },
        };
    }
}
