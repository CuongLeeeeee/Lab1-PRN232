using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "Student", Namespace = "")]
    public class StudentResponse
    {
        [DataMember(Order = 1)]
        public int StudentId { get; set; }

        [DataMember(Order = 2)]
        public string FullName { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public string Email { get; set; } = string.Empty;

        [DataMember(Order = 4)]
        public DateTime DateOfBirth { get; set; }
    }
}
