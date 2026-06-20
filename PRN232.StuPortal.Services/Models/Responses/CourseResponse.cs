using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "Course", Namespace = "")]
    public class CourseResponse
    {
        [DataMember(Order = 1)]
        public int CourseId { get; set; }

        [DataMember(Order = 2)]
        public string CourseName { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public int SemesterId { get; set; }

        // Expand: ?expand=semester
        [DataMember(Order = 4, EmitDefaultValue = false)]
        public SemesterResponse? Semester { get; set; }
    }
}
