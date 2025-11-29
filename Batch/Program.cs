using Batch.Configuration;
using Batch.Jobs;
using Batch.Models;
using Batch.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Load configuration from AWS Secrets Manager
var secretName = builder.Configuration["AWS:SecretName"] ?? "batch-app/database";
var region = builder.Configuration["AWS:Region"] ?? "ap-southeast-1";

Console.WriteLine($"Loading database credentials from AWS Secrets Manager: {secretName}");

builder.Configuration.AddAwsSecretsManager(secretName, region);

// Get connection string (built by the configuration provider)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string not configured");
}

Console.WriteLine("Database connection configured successfully");

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Register jobs
builder.Services.AddHostedService<ProcessingJob>();

var host = builder.Build();
await host.RunAsync();