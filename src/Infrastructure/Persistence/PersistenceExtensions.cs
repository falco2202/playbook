using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PlayBookDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(PlayBookDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
