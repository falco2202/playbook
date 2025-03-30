using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

    }
}
