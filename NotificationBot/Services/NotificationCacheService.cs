using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using System.Collections.Concurrent;

namespace NotificationsBot.Services
{
    /// <summary>
    /// Сервис кеширования настроек нотификации пользователей
    /// </summary>
    public class NotificationCacheService : INotificationCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AppContext _context;
        private CancellationTokenSource _cts = new();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _eventTokens = new();

        public NotificationCacheService(IMemoryCache memoryCache, AppContext context)
        {
            _memoryCache = memoryCache;
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

            List<string>? userNotifys = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SetOptions(new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });
                return await getNotificationsFromDb(chatId, project);
            });

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

            _memoryCache.Remove(cacheKey);
        }

        /// <summary>
        /// Кеширование проектов
        /// </summary>
        /// <returns></returns>
        public void CacheProjects(List<string> projects)
        {
            _memoryCache.Set(nameof(Projects), projects, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });
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
            if (!_memoryCache.TryGetValue(nameof(Projects), out List<string>? projects))
            {
                projects = await _context.Projects.Select(x => x.Name).ToListAsync();

                CacheProjects(projects);
            }

            return projects ?? [];
        }

        /// <summary>
        /// Получение или кеширование типа нотификации
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<NotificationTypes?> GetOrCacheNotificationType(string notificationType)
        {
            if (!_memoryCache.TryGetValue(notificationType, out NotificationTypes? type))
            {
                type = await _context.NotificationTypes.FirstOrDefaultAsync(nt => nt.EventDescription == notificationType.Trim());

                _memoryCache.Set(notificationType, type);
            }

            return type;
        }

        /// <summary>
        /// Получение или кеширование проекта
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Projects?> GetOrCacheProject(string project)
        {
            if (!_memoryCache.TryGetValue(project, out Projects? _project))
            {
                _project = await _context.Projects.FirstOrDefaultAsync(p => p.Name == project);

                _memoryCache.Set(project, _project);
            }

            return _project;
        }

        /// <summary>
        /// Метод удаления кеша пользователей на эвентах
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="project"></param>
        public void RemoveEventCache(string eventType, string project)
        {
            string key = $"{eventType}_{project}";

            foreach (KeyValuePair<string, CancellationTokenSource> eventCache in _eventTokens.Where(x => x.Key.Contains(key)))
            {
                if (_eventTokens.TryRemove(eventCache.Key, out CancellationTokenSource? oldCt))
                {
                    oldCt.Cancel();
                    oldCt.Dispose();
                }
            }
        }

        /// <summary>
        /// Установка или получение токена сброса отфильтрованных пользователей
        /// </summary>
        /// <returns></returns>
        public CancellationToken GetOrCreateResetToken(string key)
        {
            if (!_eventTokens.TryGetValue(key, out CancellationTokenSource? source))
            {
                source = _cts;
                _eventTokens.TryAdd(key, source);
            }

            return source.Token;
        }
    }
}
