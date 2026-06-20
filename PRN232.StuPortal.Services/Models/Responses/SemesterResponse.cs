using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "Semester", Namespace = "")]
    public class SemesterResponse
    {
        [DataMember(Order = 1)]
        public int SemesterId { get; set; }

        [DataMember(Order = 2)]
        public string SemesterName { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public DateTime StartDate { get; set; }

        [DataMember(Order = 4)]
        public DateTime EndDate { get; set; }
    }
}
