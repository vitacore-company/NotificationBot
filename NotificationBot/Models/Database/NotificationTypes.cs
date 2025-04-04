namespace NotificationsBot.Models.Database
{
    public class NotificationTypes
    {
        public int Id { get; set; }
        public required string EventType { get; set; }
        public required string EventDescription { get; set; }
    }
}
