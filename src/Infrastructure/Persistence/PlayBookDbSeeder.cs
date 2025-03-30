using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class PlayBookDbSeeder
    {
        private readonly PlayBookDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<PlayBookDbSeeder> _logger;

        public PlayBookDbSeeder(
            PlayBookDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<PlayBookDbSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAllAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedUsersAsync();
                // Add other seed methods as needed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (_context.Roles.Any()) return;

            _logger.LogInformation("Seeding roles...");

            var roles = new List<IdentityRole>
        {
            new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Name = "User", NormalizedName = "USER" },
            new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" }
        };

            foreach (var role in roles)
            {
                await _roleManager.CreateAsync(role);
            }

            _logger.LogInformation("Roles seeded successfully.");
        }

        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any()) return;

            _logger.LogInformation("Seeding users...");

            // Admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            await _userManager.CreateAsync(adminUser, "Admin123!");
            await _userManager.AddToRoleAsync(adminUser, "Admin");

            // Regular user
            var regularUser = new ApplicationUser
            {
                UserName = "user@example.com",
                NormalizedUserName = "USER@EXAMPLE.COM",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            await _userManager.CreateAsync(regularUser, "User123!");
            await _userManager.AddToRoleAsync(regularUser, "User");

            // Manager user
            var managerUser = new ApplicationUser
            {
                UserName = "manager@example.com",
                NormalizedUserName = "MANAGER@EXAMPLE.COM",
                Email = "manager@example.com",
                NormalizedEmail = "MANAGER@EXAMPLE.COM",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            await _userManager.CreateAsync(managerUser, "Manager123!");
            await _userManager.AddToRoleAsync(managerUser, "Manager");

            _logger.LogInformation("Users seeded successfully.");
        }

        // Add more seed methods for your other entities
        
    }
}
