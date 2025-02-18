namespace NotificationsBot.Models;

/// <summary>
/// Настройки клиента телеграм
/// </summary>
public sealed class TelegramBotClientOptions
{
    public TelegramBotClientOptions()
    {
    }

    /// <summary>
    /// Токен доступа для телеграмм бота.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Используется для изменения базового url на URL вашего частного api сервера бота. Он выглядит как http://localhost:8081. Путь, запрос и фрагмент будут опущены, если они присутствуют.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Указывает, что будет использоваться тестовая среда
    /// </summary>
    public bool UseTestEnvironment { get; set; }

    /// <summary>
    /// Автоматическое повторение неудачных запросов "Too Many Requests: retry after X", когда X меньше или равно RetryThreshold
    /// </summary>
    public int RetryThreshold { get; set; } = 60;

    /// <summary>
    /// <see cref="RetryThreshold">Автоматическое повторение</see> будет предпринято для запросов до RetryCount
    /// </summary>
    public int RetryCount { get; set; } = 3;
}
