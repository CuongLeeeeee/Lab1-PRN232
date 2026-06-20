using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class UpdateEnrollmentRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
