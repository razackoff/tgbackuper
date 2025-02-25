namespace TelegramBackupClient.Services;

public class BackupService : IBackupService
{
    private readonly ITelegramService _telegramService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<BackupService> _logger;

    public BackupService(
        ITelegramService telegramService,
        IFileStorageService fileStorageService,
        ILogger<BackupService> logger)
    {
        _telegramService = telegramService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task StartBackupAsync(long chatId)
    {
        try
        {
            var messages = await _telegramService.GetMessagesAsync(chatId, DateTime.UtcNow.AddDays(-1));
            foreach (var message in messages)
            {
                await _fileStorageService.SaveMessageAsync(message);
                if (!string.IsNullOrEmpty(message.MediaUrl))
                {
                    await _telegramService.DownloadMediaAsync(message.MediaUrl, 
                        _fileStorageService.GetMediaPath(message));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup failed for chat {ChatId}", chatId);
            throw;
        }
    }

    // Other implementation methods...
} 