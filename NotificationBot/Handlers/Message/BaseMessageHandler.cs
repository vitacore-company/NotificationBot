using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.Services.Common;
using NotificationsBot.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions;

namespace NotificationsBot.Handlers
{
    public abstract class BaseMessageHandler
    {
        protected readonly AppContext _context;
        protected readonly ITelegramBotClient _botClient;
        protected readonly IUserHolder _userHolder;
        protected readonly ILogger<BaseMessageHandler> _logger;

        /// <summary>
        /// Кеширование типа нотификации и проекта
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Сервис кеширования отфильтрованных данных пользователей
        /// </summary>
        private readonly INotificationCacheService _notificationCache;

        protected BaseMessageHandler(AppContext context, ITelegramBotClient botClient,
            IUserHolder userHolder, ILogger<BaseMessageHandler> logger, IMemoryCache memoryCache, INotificationCacheService notificationCache)
        {
            _context = context;
            _botClient = botClient;
            _userHolder = userHolder;
            _logger = logger;
            _memoryCache = memoryCache;
            _notificationCache = notificationCache;
        }

        /// <summary>
        /// Получение отфильтрованных пользователей по проекту и типу нотификации
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="project"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        protected async Task<Dictionary<long, int?>> FilteredByNotifyUsers(string eventType, string project, List<long> users)
        {
            if (users.Count == 0)
            {
                return [];
            }

            string cacheKey = $"filtered_users_{eventType}_{project}_{string.Join("", users)}";

            Dictionary<long, int?>? filteredUsers = await _memoryCache.GetOrCreateAsync<Dictionary<long, int?>>(cacheKey, async entry =>
                {
                    entry.AddExpirationToken(new CancellationChangeToken(_notificationCache.GetOrCreateResetToken(cacheKey)));
                    entry.AddExpirationToken(getEventToken(cacheKey));
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(3));

                    // Получение или кеширование типа нотификации и проекта
                    if (!_memoryCache.TryGetValue((eventType, project), out (int?, int?) ids))
                    {
                        ids = await getNotificationAndProjectIds(eventType, project);
                        _memoryCache.Set((eventType, project), ids);
                    }

                    (int? notificationTypeId, int? projectId) = ids;

                    if (notificationTypeId <= 0 || projectId <= 0)
                    {
                        return [];
                    }

                    Dictionary<long, int?> filtered = await getFilteredUsersAndGroups(notificationTypeId.GetValueOrDefault(), projectId.GetValueOrDefault(), users);

                    return filtered;
                });

            if (filteredUsers == null || filteredUsers.Count == 0)
            {
                return [];
            }

            _logger.LogInformation($"Получение пользователей для эвента {eventType}, проект {project}: {string.Join(',', filteredUsers)}");

            return filteredUsers;
        }

        /// <summary>
        /// Получение токена для кеширования пользователей по проекту и эвенту
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        private IChangeToken getEventToken(string tokenKey)
        {
            CancellationTokenSource ct = new CancellationTokenSource();

            IChangeToken? token = _memoryCache.GetOrCreate(tokenKey, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(3));
                return new CancellationChangeToken(ct.Token);
            });

            return token ?? new CancellationChangeToken(ct.Token);
        }

        /// <summary>
        /// Метод получения идентификаторов типа оповещения и проекта
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        private async Task<(int?, int?)> getNotificationAndProjectIds(string eventType, string project)
        {
            int? notificationTypeId = await _context.NotificationTypes
                .Where(x => x.EventType == eventType)
                .Where(x => x.Projects.Any(p => p.Name == project))
                .Select(x => (int?)x.Id)
                .SingleOrDefaultAsync();

            _ = await checkIsProjectUnknown(project);

            int? projectId = await _context.Projects
                .Where(x => x.Name == project)
                .Select(x => (int?)x.Id)
                .SingleOrDefaultAsync();

            return (notificationTypeId, projectId);
        }

        /// <summary>
        /// Проверка на существование такого проекта - если нет, то добавляем
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        private async Task<bool> checkIsProjectUnknown(string project)
        {
            if (!_context.Projects.Any(x => x.Name == project))
            {
                // получение всех типов оповещений, которые не начинаются как АКУЗовские - не с \\
                List<Models.Database.NotificationTypes> notificationTypes = _context.NotificationTypes.Where(x => !x.EventType.StartsWith("\\\\")).ToList();

                await _context.Projects.AddAsync(new Models.Database.Projects() { Name = project, NotificationTypes = notificationTypes });

                await _context.SaveChangesAsync();
            }

            return true;
        }

        /// <summary>
        /// Метод фильтрации пользователей и групп по типу оповещения и проекту
        /// </summary>
        /// <param name="notificationTypeId"></param>
        /// <param name="projectId"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        private async Task<Dictionary<long, int?>> getFilteredUsersAndGroups(int notificationTypeId, int projectId, List<long> users)
        {
            Dictionary<long, int?> filteredUsers = await _context.NotificationsOnProjectChat
                .Where(x => x.NotificationTypesId == notificationTypeId && x.ProjectId == projectId)
                .Where(user => users.Contains(user.Users.ChatId))
                .Select(x => x.Users.ChatId)
                .ToDictionaryAsync(x => x, x => (int?)null);

            Dictionary<long, int?> groupChats = await getFilteredGroupChats(notificationTypeId, projectId);
            filteredUsers.AddRange(groupChats);

            return filteredUsers;
        }

        /// <summary>
        /// Метод отправки сообщений
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="chats"></param>
        protected void SendMessages(StringBuilder sb, Dictionary<long, int?> chats)
        {
            if (chats.Count == 0)
            {
                return;
            }

            string message = sb.ToString();
            List<Task> sendTasks = new List<Task>();

            foreach ((long chatId, int? threadId) in chats)
            {
                sendTasks.Add(_botClient.SendMessage(
                    chatId,
                    message,
                    Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                    messageThreadId: threadId > 0 ? threadId : null));
            }

            // Отправляем и забываем про него, отработает само, если нет, придет на ошибку
            _ = Task.WhenAll(sendTasks).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError(t.Exception, "Ошибка отправки сообщения");
                }
            });
        }

        /// <summary>
        /// Получает групповые чаты, куда нужно отправить сообщения
        /// </summary>
        /// <param name="notificationTypeId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private async Task<Dictionary<long, int?>> getFilteredGroupChats(int notificationTypeId, int projectId)
        {
            IQueryable<int> topicIds = _context.Topics
                .Where(x => x.ProjectsId == projectId)
                .Select(x => x.Id);

            if (topicIds.Count() > 0)
            {
                Dictionary<long, int?> groupChats = await _context.NotificationsOnProjectChat
                .Include(x => x.Users)
                .ThenInclude(x => x.Topics)
                .Where(x => x.NotificationTypesId == notificationTypeId && x.ProjectId == projectId)
                .Where(x => x.Users.ChatId < 0 && x.Users.Topics.Count > 0)
                .Select(x => new
                {
                    ChatId = x.Users.ChatId,
                    TopicId = x.Users.Topics.Select(t => t.Id).FirstOrDefault(id => topicIds.Contains(id))
                })
                .ToDictionaryAsync(x => x.ChatId, x => (int?)x.TopicId);

                return groupChats;
            }

            return [];
        }

        /// <summary>
        /// Получение ссылки из markdown текста вида [title](link)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected string GetLinkFromMarkdown(string message)
        {
            Regex rg = new Regex("(?:__|[*#])|\\[(.*?)\\]\\(.*?\\)");

            Match match = rg.Match(message);

            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Приводит сообщение к формату Markdown для телеграмма (экранирует)
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        protected string FormatMarkdownToTelegram(string markdown)
        {
            return Markdown.Escape(markdown);
        }
    }
}