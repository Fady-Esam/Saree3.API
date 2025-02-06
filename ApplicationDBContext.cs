using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saree3.API.Domains;

namespace Saree3.API
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure one-to-many relationship between User and RefreshToken
            builder.Entity<UserRefreshToken>()
                .HasOne(r => r.User)
                .WithMany()  // User can have many refresh tokens
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete, if user is deleted, so are their refresh tokens


        }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    }




}
