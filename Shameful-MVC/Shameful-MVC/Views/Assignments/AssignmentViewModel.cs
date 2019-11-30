using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shameful_MVC.Views.Assignments
{
    public class AssignmentViewModel
    {
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 5)]
        public string Name { get; set; }

        [Required(ErrorMessage = "File field is required.")]
        [DataType(DataType.Upload)]
        [Utilities.FileExtensions(errorMessage: "This filed is required and only .zip files can be uploaded.", fileExtensions: "zip")]
        public IFormFile FormFile { get; set; }
    }
}