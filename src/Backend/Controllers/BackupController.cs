namespace TelegramBackupClient.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackupController : ControllerBase
{
    private readonly IBackupService _backupService;
    private readonly ILogger<BackupController> _logger;

    public BackupController(IBackupService backupService, ILogger<BackupController> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    [HttpPost("start/{chatId}")]
    public async Task<IActionResult> StartBackup(long chatId)
    {
        try
        {
            await _backupService.StartBackupAsync(chatId);
            return Ok(new { message = "Backup started successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting backup");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("status/{chatId}")]
    public async Task<IActionResult> GetStatus(long chatId)
    {
        try
        {
            var status = await _backupService.GetBackupStatusAsync(chatId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backup status");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
} 