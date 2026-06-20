using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "User", Namespace = "")]
    public class UserResponse
    {
        [DataMember(Order = 1)]
        public int UserId { get; set; }

        [DataMember(Order = 2)]
        public string Username { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public string Role { get; set; } = string.Empty;
    }
}
