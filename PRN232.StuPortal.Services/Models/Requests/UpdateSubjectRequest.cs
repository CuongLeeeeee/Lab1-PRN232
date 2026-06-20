using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class UpdateSubjectRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z]{2,6}\d{3,4}$", ErrorMessage = "SubjectCode format is invalid (e.g. PRN232).")]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Credit { get; set; }
    }
}
