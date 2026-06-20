using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "StudentV2", Namespace = "")]
    public class StudentV2Response
    {
        [DataMember(Order = 1)]
        public int StudentId { get; set; }

        [DataMember(Order = 2)]
        public string FullName { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public string Email { get; set; } = string.Empty;

        [DataMember(Order = 4)]
        public DateTime DateOfBirth { get; set; }

        [DataMember(Order = 5)]
        public string DisplayLabel { get; set; } = string.Empty;

        [DataMember(Order = 6)]
        public string ApiVersion { get; set; } = "2.0";
    }
}
