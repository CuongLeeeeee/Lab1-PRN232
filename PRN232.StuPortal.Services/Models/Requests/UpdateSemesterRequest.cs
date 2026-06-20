using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class UpdateSemesterRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string SemesterName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
