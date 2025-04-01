namespace NotificationsBot.Models.Database
{
    public class NotificationsOnProjectChat
    {
        public long UserId { get; set; }
        public User Users { get; set; }
        public int ProjectId { get; set; }
        public Projects Project { get; set; }
        public int NotificationTypesId { get; set; }
        public NotificationTypes NotificationTypes {get;set;}
    }
}
