using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task UpdateAsync(ApplicationUser user);
    }
}
