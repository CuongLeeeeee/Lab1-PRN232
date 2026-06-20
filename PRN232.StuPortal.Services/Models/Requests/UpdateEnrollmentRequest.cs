using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class UpdateEnrollmentRequest
    {
        [Required]
        [StringLength(20)]
        [RegularExpression(@"^(Active|Completed|Dropped|Pending)$",
            ErrorMessage = "Status must be Active, Completed, Dropped, or Pending.")]
        public string Status { get; set; } = string.Empty;
    }
}
