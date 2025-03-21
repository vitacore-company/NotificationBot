using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using NotificationsBot.Models.AzureModels.BuildStateChanged;
using NotificationsBot.Models.AzureModels.PullRequestComment;
using NotificationsBot.Models.AzureModels.PullRequestCreated;
using NotificationsBot.Models.AzureModels.WorkItemCreated;
using NotificationsBot.Utils;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using User = NotificationsBot.Models.User;

namespace NotificationsBot.Interfaces.Impl;

/// <summary>
/// Сервис уведомления для телеграмм бота
/// </summary>
/// <seealso cref="NotificationsBot.Interfaces.INotificationService" />
public class TelegramNotificationService : INotificationService
{
    private readonly AppContext _appContext;
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramNotificationService(AppContext appContext, ITelegramBotClient telegramBotClient)
    {
        _appContext = appContext;
        _telegramBotClient = telegramBotClient;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="eventNotification">Уведомление о событии.</param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/azure/devops/service-hooks/events?view=azure-devops">Типы событий</see>
    /// </remarks>
    public Task Notify(Event eventNotification)
    {
        if (eventNotification.Resource == null)
        {
            return Task.CompletedTask;
        }
        string resultMessage = string.Empty;
        switch (eventNotification.EventType)
        {
            case "git.push"://Code is pushed to a Git repository.
                break;
            case "git.pullrequest.created"://A pull request is created in a Git repository.
                PullRequestCreatedResource resource = ((JsonElement)eventNotification.Resource).Deserialize<PullRequestCreatedResource>() ??
                    throw new InvalidOperationException(); ;
                PullRequestCreatedNotify(resource, eventNotification.DetailedMessage.Markdown);
                break;
            case "git.pullrequest.merge.attempted"://A pull request merge is attempted in a Git repository.
                break;
            case "git.pullrequest.approved":// A merge commit is approved on a pull request.
                break;
            case "git.pullrequest.updated"://Pull request updated
                PullRequestCreatedNotify(
                    ((JsonElement)eventNotification.Resource).Deserialize<PullRequestCreatedResource>() ??
                    throw new InvalidOperationException(),
                    eventNotification.DetailedMessage.Markdown
                    );
                break;
            case "ms.vss-code.git-pullrequest-comment-event"://Pull request commented on
                PullRequestCommentNotify(
                    ((JsonElement)eventNotification.Resource).Deserialize<PullRequestCommentedResource>() ??
                    throw new InvalidOperationException(),
                    eventNotification.DetailedMessage.Markdown
                    );
                break;
            case "workitem.created"://Work item created
                WorkItemCreatedNotify(
                    JsonConvert.DeserializeObject<WorkItemCreatedResource>(eventNotification.Resource.ToString()),
                    eventNotification.DetailedMessage.Markdown
                    );
                break;
            case "workitem.deleted"://Work item deleted
                break;
            case "workitem.restored"://A work item is newly restored.
                break;
            case "workitem.updated"://A work item is changed.
                break;
            case "workitem.commented":// A work item is commented on.
                break;
            case "build.complete": // build stage changed (success, fail)
                {
                    BuildStateChangedNotify(
                    JsonConvert.DeserializeObject<BuildStateChangedResource>(eventNotification.Resource.ToString()),
                    eventNotification.DetailedMessage.Markdown
                    );
                }
                break;
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о создании PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task PullRequestCreatedNotify(PullRequestCreatedResource resource, string message)
    {
        HashSet<string> users = resource.reviewers.Select(reviewer => reviewer.uniqueName)?.ToHashSet() ?? new HashSet<string>();
        users.Add(resource.createdBy.uniqueName);
        List<long> chatIds = GetChatIds(users.ToList());
        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, FormatMarkdownToTelegram(message), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о комментарии в PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task PullRequestCommentNotify(PullRequestCommentedResource resource, string message)
    {
        HashSet<string> users = resource.pullRequest.reviewers.Select(reviewer => reviewer.uniqueName)?.ToHashSet() ?? new HashSet<string>();
        users.Add(resource.comment.author.uniqueName);
        users.Add(resource.pullRequest.createdBy.uniqueName);
        List<long> chatIds = GetChatIds(users.ToList());
        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, FormatMarkdownToTelegram(message), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о создании рабочего элемента.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task WorkItemCreatedNotify(WorkItemCreatedResource resource, string message)
    {
        HashSet<string> users = new HashSet<string>();
        if (!resource.Fields.TryGetValue("uniqueName", out object user))
        {
            if (!resource.Fields.ContainsKey("System.AssignedTo"))
            {
                return Task.CompletedTask;
            }

            user = Utilites.GetUniqueUser(resource.Fields["System.AssignedTo"]);
        }

        users.Add(user.ToString());
        List<long> chatIds = GetChatIds(users.ToList());
        _telegramBotClient.SendMessage(chatIds.First(), FormatMarkdownToTelegram(message), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Оповещения о смене состояния сборки
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task BuildStateChangedNotify(BuildStateChangedResource resource, string message)
    {
        HashSet<string> users = new HashSet<string>();

        users.Add(resource.requestedBy.uniqueName);
        List<long> chatIds = GetChatIds(users.ToList());
        _telegramBotClient.SendMessage(chatIds.First(), FormatMarkdownToTelegram(message), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);

        return Task.CompletedTask;
    }

    private long GetChatId(string displayName)
    {
        User? user = _appContext.Users.FirstOrDefault(user => user.Login == displayName);
        if (user == null)
            throw new NullReferenceException("Пользователь не найден");
        return user.ChatId;
    }

    private List<long> GetChatIds(List<string> displayNames)
    {
        List<User> users = _appContext.Users.Where(user => displayNames.Contains(user.Login ?? string.Empty)).ToList();
        if (users.Count == 0)
            return new List<long>();
        return users.Select(user => user.ChatId).ToList();
    }

    /// <summary>
    /// Экранирует служебные символы, т.к. в телеграмм не имеет полной поддержки всех Markdown символов
    /// </summary>
    /// <param name="markdown">Markdown-сообщение.</param>
    /// <returns></returns>
    private static string FormatMarkdownToTelegram(string markdown)
    {
        StringBuilder escaped = new StringBuilder();
        foreach (char ch in markdown)
        {
            if (ch >= 1 && ch <= 126)
            {
                escaped.Append('\\');
            }
            escaped.Append(ch);
        }

        return escaped.ToString();
    }
}


