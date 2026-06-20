using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "Enrollment", Namespace = "")]
    public class EnrollmentResponse
    {
        [DataMember(Order = 1)]
        public int EnrollmentId { get; set; }

        [DataMember(Order = 2)]
        public int StudentId { get; set; }

        [DataMember(Order = 3)]
        public int CourseId { get; set; }

        [DataMember(Order = 4)]
        public DateTime EnrollDate { get; set; }

        [DataMember(Order = 5)]
        public string Status { get; set; } = string.Empty;

        // Expand: ?expand=student,course
        [DataMember(Order = 6, EmitDefaultValue = false)]
        public StudentResponse? Student { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false)]
        public CourseResponse? Course { get; set; }
    }
}
