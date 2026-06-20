using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "Subject", Namespace = "")]
    public class SubjectResponse
    {
        [DataMember(Order = 1)]
        public int SubjectId { get; set; }

        [DataMember(Order = 2)]
        public string SubjectCode { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public string SubjectName { get; set; } = string.Empty;

        [DataMember(Order = 4)]
        public int Credit { get; set; }
    }
}
