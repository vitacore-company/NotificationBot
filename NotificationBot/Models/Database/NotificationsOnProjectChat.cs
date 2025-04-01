using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationsBot.Models.Database
{
    [PrimaryKey(nameof(UserId), nameof(ProjectId), nameof(NotificationTypesId))]
    public class NotificationsOnProjectChat
    {
        [Column("ChatId")]
        public long UserId { get; set; }
        public User Users { get; set; }
        public int ProjectId { get; set; }
        public Projects Project { get; set; }
        public int NotificationTypesId { get; set; }
        public NotificationTypes NotificationTypes {get;set;}
    }
}
