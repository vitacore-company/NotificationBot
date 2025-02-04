namespace NotificationsBot.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Login { get; set; }
    public required long ChatId { get; set; }
}
