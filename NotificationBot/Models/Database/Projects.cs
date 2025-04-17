using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.Database;

public class Projects
{
    [Key]
    public int Id { get; set; }

    [AllowNull]
    public string Name { get; set; }

    public List<NotificationTypes> NotificationTypes { get; set; } = new List<NotificationTypes>();

    public List<Topic> Topics { get; set; } = new List<Topic>();
}
