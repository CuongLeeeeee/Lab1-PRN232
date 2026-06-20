using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Entities
{
    public class Subject
    {
        public int SubjectId { get; set; }

        public string SubjectCode { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public int Credit { get; set; }

        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}
