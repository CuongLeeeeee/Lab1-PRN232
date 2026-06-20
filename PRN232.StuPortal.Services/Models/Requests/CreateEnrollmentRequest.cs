using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class CreateEnrollmentRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrollDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        [RegularExpression(@"^(Active|Completed|Dropped|Pending)$",
            ErrorMessage = "Status must be Active, Completed, Dropped, or Pending.")]
        public string Status { get; set; } = "Active";
    }
}
