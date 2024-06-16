using Codere_Challenge_Api.Authorization;
using Codere_Challenge_Api.Middlewares.Extensions;
using Codere_Challenge_Infrastructure.DI;
using Codere_Challenge_Services.DI;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication("ApiKey").AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", opts => opts.ApiKey = builder.Configuration.GetValue<string>("AllowedApiKey")
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServicesToDI(builder.Configuration);
builder.Services.AddServicesToDI(builder.Configuration);
builder.Services.AddJobServicesToDI(builder.Configuration);

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings());

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});

builder.Services.AddHangfireServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionMiddleware();

app.UseApiKeyMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
