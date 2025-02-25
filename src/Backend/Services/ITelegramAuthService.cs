namespace TelegramBackupClient.Services;

public interface ITelegramAuthService
{
    Task<bool> IsAuthenticatedAsync();
    Task<string> StartAuthenticationAsync(string phoneNumber);
    Task<bool> CompleteAuthenticationAsync(string phoneNumber, string code);
    Task<bool> CheckPasswordAsync(string password);
} 