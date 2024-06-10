using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Core.Settings;
using Codere_Challenge_Infrastructure.Data;
using Codere_Challenge_Infrastructure.DI;
using Codere_Challenge_Infrastructure.Repositories;
using Codere_Challenge_Jobs.Jobs;
using Codere_Challenge_Services.Implementations;
using Codere_Challenge_Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();

builder.Services.AddDbContext<TvMazeDbContext>(options =>
    options.UseSqlite("name=ConnectionStrings:DefaultConnection"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddServicesToDI(builder.Configuration);
builder.Services.AddScoped<IShowService, ShowService>();

builder.Services.Configure<JobSettings>(builder.Configuration.GetSection("JobSettings"));


// Register HttpClient and FetchShowsJob
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
