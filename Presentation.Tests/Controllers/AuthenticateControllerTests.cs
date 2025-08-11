using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Presentation.Controllers;
using Xunit;

namespace Presentation.Tests.Controllers
{
    public class AuthenticateControllerTests
    {
        private static AuthenticateController CreateController(
            Mock<IAuthenticateService> authServiceMock,
            Mock<ICookieService> cookieServiceMock,
            IConfiguration configuration)
        {
            var controller = new AuthenticateController(authServiceMock.Object, configuration, cookieServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            return controller;
        }

        private static IConfiguration BuildConfiguration(int refreshTokenExpirationInDays = 7)
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"JwtSettings:Secret", "test"},
                {"JwtSettings:Issuer", "issuer"},
                {"JwtSettings:Audience", "audience"},
                {"JwtSettings:AccessTokenExpirationInMinutes", "60"},
                {"JwtSettings:RefreshTokenExpirationInDays", refreshTokenExpirationInDays.ToString()}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task Login_ReturnsOk_WithAccessToken_AndSetsCookie()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticateService>();
            var cookieServiceMock = new Mock<ICookieService>();
            var configuration = BuildConfiguration();

            authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Success("access", "refresh"));

            var controller = CreateController(authServiceMock, cookieServiceMock, configuration);

            var request = new LoginRequest
            {
                Email = "user@test.com",
                Password = "pass"
            };

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var accessToken = okResult.Value?.GetType().GetProperty("AccessToken")?.GetValue(okResult.Value) as string;
            Assert.Equal("access", accessToken);

            cookieServiceMock.Verify(
                m => m.SetRefreshTokenCookie(
                    It.Is<HttpResponse>(r => r == controller.Response),
                    "refresh",
                    7),
                Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenAuthenticationFails()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticateService>();
            var cookieServiceMock = new Mock<ICookieService>();
            var configuration = BuildConfiguration();

            authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Failure("invalid"));

            var controller = CreateController(authServiceMock, cookieServiceMock, configuration);

            var request = new LoginRequest
            {
                Email = "user@test.com",
                Password = "wrong"
            };

            // Act
            var result = await controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
            cookieServiceMock.Verify(
                m => m.SetRefreshTokenCookie(
                    It.IsAny<HttpResponse>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task Login_ThrowsInvalidOperation_WhenJwtSettingsMissing()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticateService>();
            var cookieServiceMock = new Mock<ICookieService>();

            authServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Success("access", "refresh"));

            var configuration = new ConfigurationBuilder().Build();
            var controller = CreateController(authServiceMock, cookieServiceMock, configuration);

            var request = new LoginRequest
            {
                Email = "user@test.com",
                Password = "pass"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Login(request));
        }
    }
}

