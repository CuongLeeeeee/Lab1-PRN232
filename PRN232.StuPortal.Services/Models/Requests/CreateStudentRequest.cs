using System.ComponentModel.DataAnnotations;
using PRN232.StuPortal.Services.Validation;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class CreateStudentRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [FptuStudentCode]
        public string StudentCode { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2100")]
        public DateTime DateOfBirth { get; set; }
    }
}
