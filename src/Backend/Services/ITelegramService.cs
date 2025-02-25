namespace TelegramBackupClient.Services;

public interface ITelegramService
{
    Task<IEnumerable<TelegramMessage>> GetMessagesAsync(long chatId, DateTime since);
    Task<bool> DownloadMediaAsync(string mediaUrl, string localPath);
    Task<bool> AuthenticateAsync(string apiToken);
} 