using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Identity
{
    public partial class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PlayBookDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticateService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            PlayBookDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(string usernameOrEmail, string password)
        {
            var user = await FindByUsernameOrEmailAsync(usernameOrEmail);

            if (user == null)
                return AuthenticateResult.Failure("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return AuthenticateResult.Failure("Invalid credentials");

            // Generate tokens
            var jwtSettings = ConfigurationSettingExtensions.GetSettings<JwtSettings>(_configuration, "JwtSettings");

            var tokens = await GenerateTokensAsync(user, jwtSettings);

            return AuthenticateResult.Success(tokens.AccessToken, tokens.RefreshToken);
        }
        public async Task<AuthenticateResult> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> RevokeTokenAsync(string refreshToken, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
