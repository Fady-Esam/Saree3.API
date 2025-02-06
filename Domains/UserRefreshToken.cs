using System.ComponentModel.DataAnnotations;

namespace Saree3.API.Domains
{
    public class UserRefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => RevokedAt == null && !IsExpired;
        public string UserId { get; set; } 
        public virtual AppUser User { get; set; }
    }
}
