namespace TelegramBackupClient.Services;

public interface IBackupService
{
    Task StartBackupAsync(long chatId);
    Task<BackupStatus> GetBackupStatusAsync(long chatId);
    Task ScheduleBackupAsync(BackupSchedule schedule);
} 