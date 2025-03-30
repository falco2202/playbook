using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IAuthenticateService
    {
        Task<AuthenticateResult> AuthenticateAsync(string usernameOrEmail, string password);
        Task<AuthenticateResult> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken, string userId);
        // Task<AuthenticateResult> RegisterUserAsync(RegisterUserDto userDto);
    }
}
