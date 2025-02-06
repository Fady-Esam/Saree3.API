using Microsoft.AspNetCore.Identity;

namespace Saree3.API.Domains
{
    public class AppUser : IdentityUser
    {
        public virtual List<UserRefreshToken> UserRefreshTokens { get; set; }
        // Some Additional Properties 
    }
}
