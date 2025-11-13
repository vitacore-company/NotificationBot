using Microsoft.EntityFrameworkCore;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using NotificationTypes = NotificationsBot.Models.Database.NotificationTypes;

namespace NotificationsBot.Services
{
    /// <summary>
    /// Сервис кеширования настроек нотификации пользователей
    /// </summary>
    public class NotificationCacheService : INotificationCacheService
    {
        private readonly ICacheService _cacheService;
        private readonly AppContext _context;

        public NotificationCacheService(ICacheService cacheService, AppContext context)
        {
            _cacheService = cacheService;
            _context = context;
        }

        /// <summary>
        /// Получение настроек нотификации пользователя
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<List<string>> GetUserNotificationsAsync(long chatId, string project)
        {
            string cacheKey = $"notifications_{chatId}_{project}";

            List<string> userNotifys = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                await getNotificationsFromDb(chatId, project), new List<string>() { $"notifications_{project}" }, TimeSpan.FromMinutes(5));

            return userNotifys ?? [];
        }

        /// <summary>
        /// Удаление типа нотификации из кеша
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public void RemoveUserCacheByKey(long chatId, string project)
        {
            string cacheKey = $"notifications_{chatId}_{project}";

            _cacheService.InvalidateByKey(cacheKey);
        }

        /// <summary>
        /// Получение типов нотификации для пользователя по проекту из бд
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        private async Task<List<string>> getNotificationsFromDb(long chatId, string project)
        {
            var query = _context.Projects
                .Where(p => p.Name == project)
                .SelectMany(p => p.NotificationTypes)
                .Select(nt => new
                {
                    nt.EventDescription,

                    IsEnabled = _context.NotificationsOnProjectChat
                        .Any(n => n.UserId == chatId &&
                                 n.Project.Name == project &&
                                 n.NotificationTypesId == nt.Id)
                });

            var result = await query.ToListAsync();

            return result.Select(x =>
                (x.IsEnabled ? "✅ " : "❌ ") + x.EventDescription).ToList();
        }

        /// <summary>
        /// Достаем или кладем в кеш проекты
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllProjectsAsync()
        {
            List<string> projects = await _cacheService.GetOrCreateAsync(nameof(Projects), async () =>
                await _context.Projects.Select(x => x.Name).ToListAsync(), null, TimeSpan.FromMinutes(5));

            return projects ?? [];
        }

        /// <summary>
        /// Метод удаления кеша пользователей на эвентах
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="project"></param>
        public void RemoveEventCache(string eventType, string project)
        {
            string key = $"{eventType}_{project}";

            _cacheService.InvalidateDependencies(new List<string>() { key });
        }

        /// <summary>
        /// Получение или кеширование типа нотификации по ключу
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public async Task<NotificationTypes?> GetOrCacheNotificationType(string notificationType)
        {
            return await _cacheService.GetOrCreateAsync(notificationType, async () =>
                await _context.NotificationTypes.FirstOrDefaultAsync(nt => nt.EventDescription == notificationType.Trim()));
        }

        /// <summary>
        /// Кеширование или получение проекта по ключу
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<Projects?> GetOrCacheProject(string project)
        {
            return await _cacheService.GetOrCreateAsync(project, async () =>
                await _context.Projects.FirstOrDefaultAsync(p => p.Name == project));
        }
    }
}
