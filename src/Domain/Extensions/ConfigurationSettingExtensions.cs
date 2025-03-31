using Microsoft.Extensions.Configuration;
namespace Domain.Extensions
{
    public static class ConfigurationSettingExtensions
    {
        /// <summary>
        /// Gets and validates settings from a configuration section
        /// </summary>
        /// <typeparam name="T">The type of settings to retrieve</typeparam>
        /// <param name="configuration">The configuration instance</param>
        /// <param name="sectionName">Name of the configuration section</param>
        /// <returns>The settings object</returns>
        /// <exception cref="InvalidOperationException">Thrown when settings are missing or invalid</exception>
        public static T GetSettings<T>(this IConfiguration configuration, string sectionName) where T : class, new()
        {
            var settings = configuration.GetSection(sectionName).Get<T>() ?? 
                throw new InvalidOperationException($"{sectionName} settings are not configured properly.");

            return settings;
        }
    }
}
