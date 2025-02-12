using System.ComponentModel.DataAnnotations;

namespace NotificationsBot.Models;

public class User
{
    [Key]
    public required long ChatId { get; set; }
    public string? Login { get; set; }

    public string? State { get; set; }
}
