using efetch.Configurations;
using efetch.Providers;
using efetch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace efetch.Extensions
{
    public static class EfetchExtension
    {
        public static IServiceCollection AddEfetch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient().AddOptions().BuildServiceProvider();
            services.Configure<EfetchConfig>(configuration.GetSection("Efetch"));
            services.AddSingleton<IConsoleLoggingProvider, ConsoleLoggingProvider>();
            services.AddTransient<IEfetch, Efetch>();
            return services;
        }
    }
}
