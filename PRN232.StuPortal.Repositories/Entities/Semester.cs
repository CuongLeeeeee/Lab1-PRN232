using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Entities
{
    public class Semester
    {
        public int SemesterId { get; set; }

        public string SemesterName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}
