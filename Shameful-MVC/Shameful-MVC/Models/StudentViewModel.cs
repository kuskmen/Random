using System.ComponentModel.DataAnnotations;

namespace Shameful_MVC.Models
{
    public class StudentViewModel
    {
        [Required(ErrorMessage = "Please enter name e.g. John")]
        [StringLength(maximumLength: 20, MinimumLength = 5)]
        public string Name { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter name e.g. Doe")]
        [StringLength(maximumLength: 100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter faculty number e.g. 1234567890")]
        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string FacultyNumber { get; set; }

        [Required(ErrorMessage = "Please enter password e.g. mySecr3tPass!")]
        [StringLength(maximumLength: 30, MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please make sure passwords match.")]
        [StringLength(maximumLength: 30, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter specialty e.g. Informatics")]
        [StringLength(maximumLength: 100)]
        public string Specialty { get; set; }

        [Required]
        public byte Year { get; set; }

        [Required]
        public string FormOfEducation { get; set; }
    }
}
