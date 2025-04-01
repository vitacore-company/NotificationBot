using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsBot.Models.Database;

namespace NotificationsBot.Configuration
{
    public class NotificationsOnProjectChatConfiguration : IEntityTypeConfiguration<NotificationsOnProjectChat>
    {
        public void Configure(EntityTypeBuilder<NotificationsOnProjectChat> builder)
        {
            builder.HasKey(r => new { r.UserId, r.NotificationTypesId, r.ProjectId});
        }
    }
}
