using Microsoft.EntityFrameworkCore;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Services
{
    public class NotificationTypesService : INotificationTypesService
    {
        private readonly AppContext _context;
        private readonly INotificationCacheService _cache;

        public NotificationTypesService(AppContext context, INotificationCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Получение типов нотификации пользователя
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<List<string>> GetNotifications(long chatId, string project)
        {
            return await _cache.GetUserNotificationsAsync(chatId, project);
        }

        /// <summary>
        /// Метод установки или удаления типа нотификации по проекту для пользователя
        /// </summary>
        /// <param name="project"></param>
        /// <param name="chatId"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public async Task<List<string>> SetOrDeleteChatProjectNotification(string project, long chatId, string notificationType)
        {
            NotificationTypes? notification = await _cache.GetOrCacheNotificationType(notificationType);

            Projects? projectEntity = await _cache.GetOrCacheProject(project);

            if (notification == null || projectEntity == null)
            {
                return [];
            }

            NotificationsOnProjectChat? existing = await _context.NotificationsOnProjectChat
                .FirstOrDefaultAsync(n => n.UserId == chatId &&
                                        n.ProjectId == projectEntity.Id &&
                                        n.NotificationTypesId == notification.Id);

            if (existing != null)
            {
                _context.Remove(existing);
            }
            else
            {
                _context.Add(new NotificationsOnProjectChat
                {
                    UserId = chatId,
                    ProjectId = projectEntity.Id,
                    NotificationTypesId = notification.Id
                });
            }

            await _context.SaveChangesAsync();
            _cache.RemoveCacheByKey(chatId, project);
            _cache.RemoveEventCache(notification.EventType, project);

            return await _cache.GetUserNotificationsAsync(chatId, project);
        }

        /// <summary>
        /// Метод получения всех проектов
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetProjects()
        {
            return await _cache.GetAllProjectsAsync();
        }

        /// <summary>
        /// Метод проверки существования проекта по имени
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task<bool> IsProjectExistByName([MaybeNullWhen(false)] string projectName)
        {
            List<string> projects = await _cache.GetAllProjectsAsync();
            return projects.Contains(projectName);
        }
    }
}
