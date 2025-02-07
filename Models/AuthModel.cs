using System.Text.Json.Serialization;

namespace Saree3.API.Models
{
    public class AuthModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiresDate { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
    }
}
