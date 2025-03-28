using System.Collections.Concurrent;

namespace NotificationsBot.Models
{
    public static class LocalUsers
    {
        public static ConcurrentDictionary<string, UserInfo> Users = new ConcurrentDictionary<string, UserInfo>();
    }

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
