using System.ComponentModel.DataAnnotations;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        [StringLength(512, MinimumLength = 10)]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
