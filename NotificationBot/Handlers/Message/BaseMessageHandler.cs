using Microsoft.EntityFrameworkCore;
using NotificationsBot.Interfaces;
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
    protected async Task<List<long>> FilteredByNotifyUsers(string eventType, string project, List<long> users)
    {
        if (users.Count > 0)
        {
            int? notificationTypeId = await _context.NotificationTypes.Where(x => x.EventType == eventType)
                .Where(x => x.Projects.Any(x => x.Name == project))
                .Select(x => x.Id).SingleOrDefaultAsync();
            int? projectId = await _context.Projects.Where(x => x.Name == project).Select(x => x.Id).SingleOrDefaultAsync();

            if (notificationTypeId.HasValue && projectId.HasValue)
            {
                List<long> _users = await _context.NotificationsOnProjectChat
                    .Where(x => x.NotificationTypesId == notificationTypeId && x.ProjectId == projectId)
                    .Where(user => users.Contains(user.Users.ChatId)).Select(x => x.Users.ChatId).ToListAsync();

                _logger.LogInformation($"Получение пользователей для эвента {eventType}, проект {project}: {string.Join(',', _users)}");

                return _users;
            }
        }

        return [];
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

