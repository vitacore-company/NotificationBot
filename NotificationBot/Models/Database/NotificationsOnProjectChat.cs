using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.Database
{
    [PrimaryKey(nameof(UserId), nameof(ProjectId), nameof(NotificationTypesId))]
    public class NotificationsOnProjectChat
    {
        [Column("ChatId")]
        public long UserId { get; set; }
        [AllowNull]
        public User Users { get; set; }
        public int ProjectId { get; set; }
        [AllowNull]
        public Projects Project { get; set; }
        public int NotificationTypesId { get; set; }
        [AllowNull]
        public NotificationTypes NotificationTypes {get;set;}
    }
}
