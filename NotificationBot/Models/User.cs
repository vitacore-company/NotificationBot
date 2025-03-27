using System.ComponentModel.DataAnnotations;

namespace NotificationsBot.Models;
/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор чата.
    /// </summary>
    [Key]
    public required long ChatId { get; set; }
    /// <summary>
    /// Логин azure.
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Состояние чата пользователя
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public long UserId { get; set; }
}
