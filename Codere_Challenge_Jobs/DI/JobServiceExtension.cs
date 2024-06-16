using Codere_Challenge_Core.Settings;
using Codere_Challenge_Jobs.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codere_Challenge_Services.DI
{
    public static class JobServiceExtension
    {
        public static IServiceCollection AddJobServicesToDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJobStatusService, JobStatusService>();
            services.AddHttpClient<FetchShowsJob>();
            services.AddScoped<IFetchShowsJob, FetchShowsJob>();
            services.Configure<JobSettings>(configuration.GetSection("JobSettings"));

            return services;
        }
    }
}
