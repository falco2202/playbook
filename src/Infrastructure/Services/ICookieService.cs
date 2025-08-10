using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public interface ICookieService
    {
        void SetRefreshTokenCookie(HttpResponse response, string refreshToken, int expirationDays);
        string? GetRefreshTokenFromRequest(HttpRequest request);
        void RemoveRefreshTokenCookie(HttpResponse response);
    }
}
