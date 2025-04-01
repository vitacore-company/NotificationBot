using System.ComponentModel.DataAnnotations;

namespace NotificationsBot.Models.Database;

public class Projects
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}
