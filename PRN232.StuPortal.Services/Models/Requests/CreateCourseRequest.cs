using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class CreateCourseRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int SemesterId { get; set; }
    }
}
