using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Presentation.Tests
{
    public class CookieServiceTests
    {
        private readonly CookieService _cookieService = new();

        [Fact]
        public void GetRefreshTokenFromRequest_ReturnsNull_WhenCookieMissing()
        {
            var context = new DefaultHttpContext();
            var token = _cookieService.GetRefreshTokenFromRequest(context.Request);
            Assert.Null(token);
        }

        [Fact]
        public void GetRefreshTokenFromRequest_ReturnsToken_WhenCookieExists()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Cookie"] = "refresh-token=test-token";

            var token = _cookieService.GetRefreshTokenFromRequest(context.Request);

            Assert.Equal("test-token", token);
        }
    }
}
