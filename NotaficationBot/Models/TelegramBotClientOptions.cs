namespace NotificationsBot.Models;

public sealed class TelegramBotClientOptions
{
    public TelegramBotClientOptions()
    {
    }
    public string? Token { get; set; }
    public string? BaseUrl { get; set; }
    public bool UseTestEnvironment { get; set; }
    public int RetryThreshold { get; set; } = 60;
    public int RetryCount { get; set; } = 3;
}
