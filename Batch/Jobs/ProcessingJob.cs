using Batch.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Batch.Jobs;

public class ProcessingJob(ILogger<ProcessingJob> logger, IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly ILogger<ProcessingJob> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Processing at: {Time}", DateTime.UtcNow);
                
                using var scope = _serviceProvider.CreateAsyncScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var user = await userService.GetUserByIdAsync(1);

                _logger.LogInformation("Found user: {UserId} - {Name}", user.Id, user.username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Processing Job");
            }

            _logger.LogInformation("Waiting {Seconds} seconds before next run...", _interval.TotalSeconds);
            await Task.Delay(_interval, stoppingToken);
        }
        
        _logger.LogInformation("Processing Job stopped");
    }
}