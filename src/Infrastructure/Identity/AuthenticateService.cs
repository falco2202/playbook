using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PlayBookDbContext _context;
        private readonly IConfiguration configuration;

        public AuthenticateService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            PlayBookDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            this.configuration = configuration;
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
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
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

        #region Helpers
        private async Task<ApplicationUser> FindByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail);

            if (user == null && usernameOrEmail.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(usernameOrEmail);
            }

            return user;
        }

        private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user, JwtSettings jwtSettings)
        {
            var accessToken = await GenerateJwtTokenAsync(user, jwtSettings);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = GetJwtId(accessToken),
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationInDays)
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, JwtSettings jwtSettings)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            // Add roles to claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token, JwtSettings jwtSettings)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Important: don't validate lifetime here
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        private string GetJwtId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Id;
        }
        #endregion
    }
}
