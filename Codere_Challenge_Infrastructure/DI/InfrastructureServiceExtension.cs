using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Infrastructure.Data;
using Codere_Challenge_Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codere_Challenge_Infrastructure.DI
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddInfrastructureServicesToDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TvMazeDbContext>(opts =>
            {
                opts.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INetworkRepository, NetworkRepository>();
            services.AddScoped<IShowRepository, ShowRepository>();
            services.AddScoped<IJobExecutionRepository, JobExecutionRepository>();

            return services;
        }
    }
}
