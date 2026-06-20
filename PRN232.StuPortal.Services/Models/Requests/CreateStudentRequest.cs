using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class CreateStudentRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
    }
}
