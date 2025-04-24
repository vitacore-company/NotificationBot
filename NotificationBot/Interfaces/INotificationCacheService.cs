using NotificationsBot.Models.Database;

namespace NotificationsBot.Interfaces
{
    public interface INotificationCacheService
    {
        Task<List<string>> GetUserNotificationsAsync(long chatId, string project);
        void RemoveUserCacheByKey(long chatId, string project);
        void CacheProjects(List<string> projects);
        Task<List<string>> GetAllProjectsAsync();
        Task<NotificationTypes?> GetOrCacheNotificationType(string notificationType);
        Task<Projects?> GetOrCacheProject(string project);
        void RemoveEventCache(string eventType, string project);
        CancellationToken GetOrCreateResetToken(string key);
    }
}
