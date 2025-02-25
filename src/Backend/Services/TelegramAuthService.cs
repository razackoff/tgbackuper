using TeleSharp.TL;
using TLSharp.Core;

namespace TelegramBackupClient.Services;

public class TelegramAuthService : ITelegramAuthService
{
    private readonly TelegramClient _client;
    private readonly ILogger<TelegramAuthService> _logger;
    private readonly TelegramSettings _settings;
    private string _hash;

    public TelegramAuthService(
        ILogger<TelegramAuthService> logger,
        IOptions<TelegramSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
        _client = new TelegramClient(
            int.Parse(_settings.ApiId), 
            _settings.ApiHash,
            sessionStore: new FileSessionStore());
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            await _client.ConnectAsync();
            return _client.IsUserAuthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authentication status");
            return false;
        }
    }

    public async Task<string> StartAuthenticationAsync(string phoneNumber)
    {
        try
        {
            await _client.ConnectAsync();
            _hash = await _client.SendCodeRequestAsync(phoneNumber);
            return _hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting authentication");
            throw;
        }
    }

    public async Task<bool> CompleteAuthenticationAsync(string phoneNumber, string code)
    {
        try
        {
            var user = await _client.MakeAuthAsync(phoneNumber, _hash, code);
            return user != null;
        }
        catch (CloudPasswordNeededException)
        {
            _logger.LogInformation("Two-factor authentication required");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing authentication");
            throw;
        }
    }

    public async Task<bool> CheckPasswordAsync(string password)
    {
        try
        {
            var password2FA = await _client.Get2FAPasswordSrpAsync();
            await _client.MakeAuthWithPasswordAsync(password2FA, password);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking 2FA password");
            throw;
        }
    }
} 