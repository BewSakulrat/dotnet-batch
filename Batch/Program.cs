using Batch.Configuration;
using Batch.Jobs;
using Batch.Models;
using Batch.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();
var environment = builder.Environment.EnvironmentName;

Console.WriteLine($"Environment: {environment}");

string? connectionString;

if (isDevelopment)
{
    // Use local appsettings.json
    Console.WriteLine("📍 Development mode: Using local configuration");
    
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string not found in appsettings.json");
    }
    
    Console.WriteLine("Connection string loaded from appsettings.json");
}
else
{
    // Load from AWS Secrets Manager
    Console.WriteLine("🔐 Production mode: Loading from AWS Secrets Manager");
    
    var secretName = builder.Configuration["AWS:SecretName"] ?? "batch-app/database";
    var region = builder.Configuration["AWS:Region"] ?? "ap-southeast-1";
    
    Console.WriteLine($"Loading secrets from: {secretName} ({region})");
    
    builder.Configuration.AddAwsSecretsManager(secretName, region);
    
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string not found in AWS Secrets Manager");
    }
    
    Console.WriteLine("Connection string loaded from AWS Secrets Manager");
}

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Configure Quartz.NET
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("UserProcessingJob");
    
    q.AddJob<SchedulerJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("UserProcessingJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(10)
            .RepeatForever()));
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var host = builder.Build();

Console.WriteLine("🚀 Application starting...");

await host.RunAsync();