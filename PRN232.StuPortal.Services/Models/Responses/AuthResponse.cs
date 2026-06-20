using System.Runtime.Serialization;

namespace PRN232.StuPortal.Services.Models.Responses
{
    [DataContract(Name = "AuthResponse", Namespace = "")]
    public class AuthResponse
    {
        [DataMember(Order = 1)]
        public string AccessToken { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string RefreshToken { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public int ExpiresIn { get; set; }
    }
}
