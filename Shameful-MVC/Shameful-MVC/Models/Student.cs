using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shameful_MVC.Models
{
    public class Student
    {
        [BindProperty]
        [Required(ErrorMessage = "Please enter name e.g. John")]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        public string Name { get; set; }

        public string MiddleName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter name e.g. Doe")]
        [StringLength(maximumLength: 100)]
        public string LastName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter faculty number e.g. 1234567890")]
        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string FacultyNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter password e.g. mySecr3tPass!")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 30, MinimumLength = 6)]
        [RegularExpression("")]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Make sure both passwords are the same")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter specialty e.g. Informatics")]
        [StringLength(maximumLength: 100)]
        public string Specialty { get; set; }

        [BindProperty]
        [Required]
        public byte Year { get; set; }

        [BindProperty]
        [Required]
        public string FormOfEducation { get; set; }
    }
}
