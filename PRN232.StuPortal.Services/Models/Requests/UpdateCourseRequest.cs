using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Services.Models.Requests
{
    public class UpdateCourseRequest
    {
        [Required]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public int SemesterId { get; set; }
    }
}
