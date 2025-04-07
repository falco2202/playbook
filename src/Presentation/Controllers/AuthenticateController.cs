using Application.Common.Interfaces;
using Domain.Extensions;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        private readonly ICookieService _cookieService;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            IAuthenticateService authService, 
            IConfiguration configuration,
            ICookieService cookieService)
        {
            _authService = authService;
            _cookieService = cookieService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.AuthenticateAsync(request.Email, request.Password);

            if (!result.Succeeded)
                return Unauthorized(result.Errors);

            var jwtSettings = ConfigurationSettingExtensions.GetSettings<JwtSettings>(_configuration, "JwtSettings");

            if (jwtSettings == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "JWT settings are not configured properly.");

            _cookieService.SetRefreshTokenCookie(Response, result.RefreshToken, jwtSettings.RefreshTokenExpirationInDays);

            return Ok(new
            {
                result.AccessToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            // Get access token from Authorization header
            var accessToken = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized(new { message = "Access token is required" });

            // Get refresh token from cookie (browser sends automatically)
            var refreshToken = _cookieService.GetRefreshTokenFromRequest(Request);
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token is missing" });

            var result = await _authService.RefreshTokenAsync(accessToken, refreshToken);
            if (!result.Succeeded)
                return Unauthorized(new { message = result.Errors.FirstOrDefault() });

            var jwtSettings = ConfigurationSettingExtensions.GetSettings<JwtSettings>(_configuration, "JwtSettings");

            if (jwtSettings == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "JWT settings are not configured properly.");

            // Set new refresh token in cookie
            _cookieService.SetRefreshTokenCookie(Response, result.RefreshToken, jwtSettings.RefreshTokenExpirationInDays);

            // Return new access token in response body
            return Ok(new { result.AccessToken });
        }

        [HttpPost]
        public async Task<IActionResult> Register()
        {
            throw new NotImplementedException();
        }
    }

}
