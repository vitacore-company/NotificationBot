namespace NotificationsBot.Models.Locals
{
    public struct UserInfo
    {
        public UserInfo(long chatId, bool available)
        {
            ChatId = chatId;
            Available = available;
        }

        public long ChatId { get; set; }

        public bool Available { get; set; }
    }
}
