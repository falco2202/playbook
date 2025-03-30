using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class PlayBookDbContext : IdentityDbContext<ApplicationUser>
    {
        public PlayBookDbContext(DbContextOptions<PlayBookDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize Identity tables if needed
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("Roles");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired();
                entity.Property(rt => rt.JwtId).IsRequired();
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.ApplyConfigurationsFromAssembly(typeof(PlayBookDbContext).Assembly);
        }
    }
}
