using System.Collections.Concurrent;

namespace NotificationsBot.Models.Locals
{
    public static class LocalMessageHandlers
    {
        public static ConcurrentDictionary<string, Type> Handlers = new ConcurrentDictionary<string, Type>();
    }
}
