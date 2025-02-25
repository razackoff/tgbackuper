namespace TelegramBackupClient.Models;

public class TelegramMessage
{
    public long MessageId { get; set; }
    public long ChatId { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public string? MediaType { get; set; }
    public string? MediaUrl { get; set; }
    public User Sender { get; set; }
}

public class User
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
} 