using System.ComponentModel.DataAnnotations;

namespace NotificationsBot.Models;

public class AppVersion
{
    [Key]
    [MaxLength(10)]
    public string Version { get; set; }
}
