using Application.Common.Events;
using Application.Common.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddScoped<ICookieService, CookieService>(); 

            return services;
        }
    }
}
