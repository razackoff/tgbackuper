namespace TelegramBackupClient.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITelegramAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ITelegramAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetAuthStatus()
    {
        var isAuthenticated = await _authService.IsAuthenticatedAsync();
        return Ok(new { isAuthenticated });
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartAuth([FromBody] StartAuthRequest request)
    {
        try
        {
            var hash = await _authService.StartAuthenticationAsync(request.PhoneNumber);
            return Ok(new { message = "Code sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting authentication");
            return StatusCode(500, new { error = "Failed to start authentication" });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
    {
        try
        {
            var success = await _authService.CompleteAuthenticationAsync(
                request.PhoneNumber, 
                request.Code);
            
            return Ok(new { success });
        }
        catch (CloudPasswordNeededException)
        {
            return StatusCode(403, new { error = "2FA required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying code");
            return StatusCode(500, new { error = "Failed to verify code" });
        }
    }

    [HttpPost("2fa")]
    public async Task<IActionResult> Check2FA([FromBody] Check2FARequest request)
    {
        try
        {
            var success = await _authService.CheckPasswordAsync(request.Password);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking 2FA");
            return StatusCode(500, new { error = "Failed to verify 2FA" });
        }
    }
}

public class StartAuthRequest
{
    public string PhoneNumber { get; set; }
}

public class VerifyCodeRequest
{
    public string PhoneNumber { get; set; }
    public string Code { get; set; }
}

public class Check2FARequest
{
    public string Password { get; set; }
} 