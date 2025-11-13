using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Interfaces
{
    public interface INotificationTypesService
    {
        public Task<List<string>> GetProjects();
        public Task<List<string>> GetNotifications(long chatId, string project);
        public Task<List<string>> SetOrDeleteChatProjectNotification(string project, long chatId, string notificationType);
        public Task<bool> IsProjectExistByName([MaybeNullWhen(false)]string projectName);
    }
}
