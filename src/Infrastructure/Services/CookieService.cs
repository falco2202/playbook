using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private const string RefreshTokenKey = "refresh-token";

        public void SetRefreshTokenCookie(HttpResponse response, string refreshToken, int expirationDays)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Path = "/"
            };

            response.Cookies.Append(RefreshTokenKey, refreshToken, cookieOptions);
        }

        public string? GetRefreshTokenFromRequest(HttpRequest request)
        {
            return request.Cookies.TryGetValue(RefreshTokenKey, out var token)
                ? token
                : null;
        }

        public void RemoveRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete(RefreshTokenKey);
        }
    }
}
