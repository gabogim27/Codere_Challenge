using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Core.Settings;
using Codere_Challenge_Infrastructure.Data;
using Codere_Challenge_Infrastructure.Repositories;
using Codere_Challenge_Jobs.Jobs;
using Codere_Challenge_Services.Implementations;
using Codere_Challenge_Services.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<TvMazeDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INetworkRepository, NetworkRepository>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IJobExecutionRepository, JobExecutionRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddServicesToDI(builder.Configuration);
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IJobExecutionService, JobExecutionService>();

builder.Services.Configure<JobSettings>(builder.Configuration.GetSection("JobSettings"));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings());

// Add the processing server as IHostedService
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});

builder.Services.AddHangfireServer();

builder.Services.AddSingleton<IJobStatusService, JobStatusService>();

builder.Services.AddHttpClient<FetchShowsJob>();
builder.Services.AddScoped<IFetchShowsJob, FetchShowsJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
