using Microsoft.EntityFrameworkCore;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Services
{
    public class NotificationTypesService : INotificationTypesService
    {
        private readonly AppContext _context;
        public NotificationTypesService(AppContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetNotifications(long chatId, string project)
        {
            if (chatId != -1 && !string.IsNullOrEmpty(project))
            {
                Projects? _project = await _context.Projects.Where(x => x.Name == project).FirstOrDefaultAsync();

                if (_project != null)
                {
                    return await getNotifys(_project, chatId);
                }
            }

            return [];
        }

        public List<string> GetProjects()
        {
            return _context.Projects.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Устанавливает или удаляет тип оповещения для пользователя
        /// </summary>
        /// <param name="project"></param>
        /// <param name="chatId"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public async Task<List<string>> SetOrDeleteChatProjectNotification([AllowNull]string project, long chatId, [AllowNull]string notificationType)
        {
            if (string.IsNullOrEmpty(project) || string.IsNullOrEmpty(notificationType))
            {
                return [];
            }
            Projects? _project = await _context.Projects.Where(x => x.Name == project).FirstOrDefaultAsync();
            NotificationTypes? type = await _context.NotificationTypes.Where(x => x.EventDescription == notificationType).FirstOrDefaultAsync();

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

        public Task<bool> GetProjectByName([MaybeNullWhen(false)]string projectName)
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
            List<NotificationsOnProjectChat> notifys = await _context.NotificationsOnProjectChat.Where(x => x.UserId == chatId && x.Project == project).Include(x => x.NotificationTypes).ToListAsync();

            foreach (NotificationsOnProjectChat notify in notifys)
            {
                userNotifys.Add(char.ConvertFromUtf32(0x2714) + notify.NotificationTypes.EventDescription);
                typesWithoutEmoji.Add(notify.NotificationTypes.EventDescription);
            }

            foreach (string notify in getNotificationsTypes().Except(typesWithoutEmoji))
            {
                userNotifys.Add(char.ConvertFromUtf32(0x274C) + notify);
            }

            return userNotifys;
        }

        private List<string> getNotificationsTypes()
        {
            return _context.NotificationTypes.Select(x => x.EventDescription).ToList();
        }

    }
}
