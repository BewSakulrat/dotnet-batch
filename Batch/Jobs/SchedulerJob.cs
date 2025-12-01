using Batch.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Batch.Jobs;

public class SchedulerJob(ILogger<SchedulerJob> logger, IUserService userService) : IJob
{
    private readonly ILogger<SchedulerJob> _logger = logger;
    private readonly IUserService _userService = userService;

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Quartz job executing at: {Time}", DateTime.UtcNow);

        try
        {
            var users = await _userService.GetUserByIdAsync(1);
            
            _logger.LogInformation("Processed found {name}", users.username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Quartz job");
            throw new JobExecutionException(ex);
        }
    }
}