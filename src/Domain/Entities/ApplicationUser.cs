using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int TokenVersion { get; private set; } = 0;  // Ensures Refresh Token invalidation
        public void IncrementTokenVersion()
        {
            TokenVersion++;
        }
    }
}
