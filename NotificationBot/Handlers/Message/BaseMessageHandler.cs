using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.Database;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions;

namespace NotificationsBot.Handlers;

public abstract class BaseMessageHandler
{
    protected readonly AppContext _context;
    protected readonly ITelegramBotClient _botClient;
    protected readonly IUserHolder _userHolder;
    protected readonly ILogger<BaseMessageHandler> _logger;

    protected BaseMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger)
    {
        _context = context;
        _botClient = botClient;
        _userHolder = userHolder;
        _logger = logger;

    }

    /// <summary>
    /// Фильтрует пользователей по типу оповещения
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="project"></param>
    /// <param name="users"></param>
    /// <returns></returns>
    protected async Task<Dictionary<long, int?>> FilteredByNotifyUsers(string eventType, string project, List<long> users)
    {
        if (users.Count > 0)
        {
            int? notificationTypeId = await _context.NotificationTypes.Where(x => x.EventType == eventType)
                .Where(x => x.Projects.Any(x => x.Name == project))
                .Select(x => x.Id).SingleOrDefaultAsync();
            int? projectId = await _context.Projects.Where(x => x.Name == project).Select(x => x.Id).SingleOrDefaultAsync();

            if (notificationTypeId > 0 && projectId > 0)
            {
                Dictionary<long, int?> _users = await _context.NotificationsOnProjectChat
                     .Where(x => x.NotificationTypesId == notificationTypeId && x.ProjectId == projectId)
                     .Where(user => users.Contains(user.Users.ChatId))
                     .Select(x => x.Users.ChatId)
                     .ToDictionaryAsync(x => x, x => (int?)null);

                _users.AddRange(await filteredGroupChats(notificationTypeId, projectId));

                _logger.LogInformation($"Получение пользователей для эвента {eventType}, проект {project}: {string.Join(',', _users.Select(x => x.Key))}");

                return _users;
            }
        }

        return [];
    }

    /// <summary>
    /// Отправка сообщений пользователям
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="chats"></param>
    protected void SendMessages(StringBuilder sb, Dictionary<long, int?> chats)
    {
        string message = sb.ToString();

        foreach (KeyValuePair<long, int?> item in chats)
        {
            int? thread = null;

            if (item.Value > 0)
            {
                thread = item.Value;
            }

            _ = _botClient.SendMessage(item.Key, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, messageThreadId: thread);
        }
    }

    /// <summary>
    /// Метод, который собирает чаты с групп
    /// </summary>
    /// <param name="notificationTypeId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    private async Task<Dictionary<long, int?>> filteredGroupChats(int? notificationTypeId, int? projectId)
    {
        IQueryable<int> checkTopic = _context.Topics.Where(x => x.ProjectsId == projectId).Select(x => x.Id);

        if (checkTopic.Count() > 0)
        {
            Dictionary<long, int?> groupChats = await _context.NotificationsOnProjectChat
                .Include(x => x.Users)
                .ThenInclude(x => x.Topics)
                .Where(x => x.NotificationTypesId == notificationTypeId && x.ProjectId == projectId)
                .Where(x => x.Users.ChatId < 0 && x.Users.Topics.Count > 0)
                .Select(x => new
                {
                    ChatId = x.Users.ChatId,
                    TopicId = x.Users.Topics.Select(t => t.Id).FirstOrDefault(id => checkTopic.Contains(id)) // Находим пересечение топиков пользователя с топиками проекта
                })
            .ToDictionaryAsync(x => x.ChatId, x => (int?)x.TopicId);

            return groupChats;
        }

        return [];
    }

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
    /// Экранирует служебные символы, т.к. в телеграмм не имеет полной поддержки всех Markdown символов
    /// </summary>
    /// <param name="markdown">Markdown-сообщение.</param>
    /// <returns></returns>
    protected string FormatMarkdownToTelegram(string markdown)
    {
        return Markdown.Escape(markdown);
    }
}