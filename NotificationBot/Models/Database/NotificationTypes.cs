namespace NotificationsBot.Models.Database
{
    public class NotificationTypes
    {
        public int Id { get; set; }
        public required string EventType { get; set; }
        public required string EventDescription { get; set; }
        public List<Projects> Projects { get; set; } = new List<Projects>();
    }
}
