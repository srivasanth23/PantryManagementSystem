using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PantryManagementSystem.Data
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        // ✅ Use generic DbContextOptions<AuthDbContext>
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var AdminRoleId = "38f9b9c5-6e6a-4a01-a8da-9b31db2d28b5";
            var StaffRoleId = "f0a34ec1-36c2-48b8-9a57-de3bdcde907d";
            var UserRoleId = "418718df-9565-4ec8-bf09-be5a4fce0fd0";

            // Seed roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = AdminRoleId,
                    ConcurrencyStamp = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = StaffRoleId,
                    ConcurrencyStamp = StaffRoleId,
                    Name = "Staff",
                    NormalizedName = "STAFF"
                },
                new IdentityRole
                {
                    Id = UserRoleId,
                    ConcurrencyStamp = UserRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
