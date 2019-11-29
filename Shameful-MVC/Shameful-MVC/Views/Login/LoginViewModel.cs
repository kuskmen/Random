using Microsoft.AspNetCore.Mvc;

namespace Shameful_MVC.Views.Home
{
    public class LoginViewModel
    {
        [BindProperty]
        public string FacultyNumber { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public bool SuccessfulLogin { get; set; } = true;
    }
}
