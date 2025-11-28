using Batch.Models;
using Batch.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// DEBUG: Check if connection string is loaded
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString ?? "NULL - NOT FOUND!"}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured in appsettings.json");
}

// Register repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// If using Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();

// Run the job
using (var scope = host.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    
    try
    {
        logger.LogInformation("Starting batch job...");
        var user = await userService.GetUserByIdAsync(1);
        logger.LogInformation($"User: {user.Id}");
        logger.LogInformation("Batch job completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Batch job failed");
        Environment.Exit(1);
    }
}