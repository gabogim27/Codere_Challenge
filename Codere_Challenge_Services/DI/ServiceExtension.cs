using Codere_Challenge_Services.Implementations;
using Codere_Challenge_Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codere_Challenge_Services.DI
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServicesToDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IShowService, ShowService>();
            services.AddScoped<IJobExecutionService, JobExecutionService>();

            return services;
        }
    }
}
