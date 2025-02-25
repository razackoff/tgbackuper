namespace TelegramBackupClient.Infrastructure;

public class BackupSchedulerService : BackgroundService
{
    private readonly IBackupService _backupService;
    private readonly ILogger<BackupSchedulerService> _logger;

    public BackupSchedulerService(
        IBackupService backupService,
        ILogger<BackupSchedulerService> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Check for scheduled backups and execute them
                // Implementation details...
                
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in backup scheduler");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
} 