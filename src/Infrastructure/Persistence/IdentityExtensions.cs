using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Persistence
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<PlayBookDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            ConfigureIdentityOptions(services, configuration);
            ConfigureAuthentication(services, configuration);

            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<ICookieService, CookieService>();

            return services;
        }

        private static void ConfigureIdentityOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            // Ensure JwtSettings is not null
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtSettings configuration is missing or invalid");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
