using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Services
{
    public class NotificationTypesService : INotificationTypesService
    {
        private readonly AppContext _context;
        private readonly IMemoryCache _memoryCache;

        public NotificationTypesService(AppContext context, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _context = context;
        }

        public async Task<List<string>> GetNotifications(long chatId, string project)
        {
            if (chatId != -1 && !string.IsNullOrEmpty(project))
            {
                Projects? _project = tryGetProjectFromCacheOrArr(project);

                if (_project != null)
                {
                    return await getNotifys(_project, chatId);
                }
            }

            return [];
        }

        public List<string> GetProjects()
        {
            if (!_memoryCache.TryGetValue(nameof(Projects), out List<string>? projects))
            {
                projects = _context.Projects.Select(x => x.Name).ToList();

                _memoryCache.Set(nameof(Projects), projects, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });
            }

            return projects ?? new List<string>();
        }

        /// <summary>
        /// Устанавливает или удаляет тип оповещения для пользователя
        /// </summary>
        /// <param name="project"></param>
        /// <param name="chatId"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public async Task<List<string>> SetOrDeleteChatProjectNotification([AllowNull] string project, long chatId, [AllowNull] string notificationType)
        {
            if (string.IsNullOrEmpty(project) || string.IsNullOrEmpty(notificationType))
            {
                return [];
            }
            Projects? _project = tryGetProjectFromCacheOrArr(project);
            NotificationTypes? type = tryGetNotificationTypesFromCacheOrArr(notificationType);

            if (type != null && _project != null)
            {
                NotificationsOnProjectChat? chatNotification = await _context.NotificationsOnProjectChat
                    .Where(x => x.UserId == chatId && x.Project == _project && x.NotificationTypes == type).SingleOrDefaultAsync();

                if (chatNotification != null)
                {
                    _context.NotificationsOnProjectChat.Remove(chatNotification);
                }
                else
                {
                    NotificationsOnProjectChat newNotify = new NotificationsOnProjectChat { UserId = chatId, Project = _project, NotificationTypes = type };
                    await _context.NotificationsOnProjectChat.AddAsync(newNotify);
                }

                await _context.SaveChangesAsync();

                return await getNotifys(_project, chatId);
            }

            return [];
        }

        public Task<bool> GetProjectByName([MaybeNullWhen(false)] string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                return Task.FromResult(false);
            }
            int project = _context.Projects.Where(x => x.Name == projectName).Count();

            return Task.FromResult(project > 0);
        }

        private async Task<List<string>> getNotifys(Projects project, long chatId)
        {
            List<string> userNotifys = new List<string>();
            List<string> typesWithoutEmoji = new List<string>();

            List<NotificationsOnProjectChat> notifys = await _context.NotificationsOnProjectChat.Where(x => x.UserId == chatId && x.Project == project && project.NotificationTypes.Contains(x.NotificationTypes)).Include(x => x.NotificationTypes).ToListAsync();

            foreach (NotificationsOnProjectChat notify in notifys)
            {
                userNotifys.Add(char.ConvertFromUtf32(0x2714) + notify.NotificationTypes.EventDescription);
                typesWithoutEmoji.Add(notify.NotificationTypes.EventDescription);
            }

            foreach (string notify in project.NotificationTypes.Select(x => x.EventDescription).Except(typesWithoutEmoji))
            {
                userNotifys.Add(char.ConvertFromUtf32(0x274C) + notify);
            }

            return userNotifys;
        }

        private Projects? tryGetProjectFromCacheOrArr(string projectName)
        {
            if (!_memoryCache.TryGetValue(projectName, out Projects? project))
            {
                project = _context.Projects.Where(x => x.Name == projectName).Include(x => x.NotificationTypes).FirstOrDefault();

                _memoryCache.Set(projectName, project);
            }

            return project;
        }

        private NotificationTypes? tryGetNotificationTypesFromCacheOrArr(string notificationType)
        {
            if (!_memoryCache.TryGetValue(notificationType, out NotificationTypes? type))
            {
                type = _context.NotificationTypes.Where(x => x.EventDescription == notificationType).FirstOrDefault();

                _memoryCache.Set(notificationType, type);
            }

            return type;
        }
    }
}
