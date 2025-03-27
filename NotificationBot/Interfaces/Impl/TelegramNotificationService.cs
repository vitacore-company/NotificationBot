using Microsoft.AspNet.WebHooks.Payloads;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotificationsBot.Models.AzureModels.PullRequestComment;
using NotificationsBot.Utils;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions;

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

    public Task Notify(JsonElement element, string eventType)
    {
        try
        {
            switch (eventType)
            {
                case "git.pullrequest.updated":
                    {
                        GitPullRequestUpdatedPayload? updated = JsonConvert.DeserializeObject<GitPullRequestUpdatedPayload>(element.ToString());

                        PullRequestUpdatedNotify(updated);
                    }
                    break;

                case "ms.vss-code.git-pullrequest-comment-event":
                    {
                        PullRequestCommentedPayload? pullRequestCommented = JsonConvert.DeserializeObject<PullRequestCommentedPayload>(element.ToString());

                        PullRequestCommentNotify(pullRequestCommented);
                    }
                    break;

                case "git.pullrequest.created":
                    {
                        GitPullRequestCreatedPayload? created = JsonConvert.DeserializeObject<GitPullRequestCreatedPayload>(element.ToString());

                        PullRequestCreatedNotify(created);
                    }
                    break;

                case "workitem.created":
                    {
                        WorkItemCreatedCustomPayload? created = JsonConvert.DeserializeObject<WorkItemCreatedCustomPayload>(element.ToString());

                        WorkItemCreatedNotify(created);
                    }
                    break;

                case "workitem.updated":
                    {
                        WorkItemUpdatedCustomPayload? updated = JsonConvert.DeserializeObject<WorkItemUpdatedCustomPayload>(element.ToString());

                        WorkItemUpdatedNotify(updated);
                    }
                    break;

                case "build.complete":
                    {
                        BuildCompletedPayload? build = JsonConvert.DeserializeObject<BuildCompletedPayload>(element.ToString());

                        BuildStateChangedNotify(build);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о создании PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task PullRequestCreatedNotify(GitPullRequestCreatedPayload resource)
    {
        HashSet<string> users = resource.Resource.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
        List<long> chatIds = GetChatIds(users.ToList());

        string message = FormatMarkdownToTelegram(resource.Message.Text.Substring(0, resource.Message.Text.LastIndexOf(resource.Resource.PullRequestId.ToString())) + Environment.NewLine
            + $"Project: {resource.Resource.Repository.Name}" + Environment.NewLine +
            $"Title: {resource.Resource.Title}" + Environment.NewLine +
            $"Description: {Environment.NewLine} {resource.Resource.Description}");

        message = message.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.Repository.Name, resource.Resource.PullRequestId, "pull request"));

        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о комментарии в PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task PullRequestUpdatedNotify(GitPullRequestUpdatedPayload resource)
    {
        HashSet<string> users = resource.Resource.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();

        List<long> chatIds = GetChatIds(users.ToList());

        string message = FormatMarkdownToTelegram(resource.Message.Text.Substring(0, resource.Message.Text.LastIndexOf(resource.Resource.PullRequestId.ToString())) + Environment.NewLine
            + $"Project: {resource.Resource.Repository.Name}" + Environment.NewLine +
            $"Title: {resource.Resource.Title}" + Environment.NewLine +
            $"Description: {resource.Resource.Description}");

        message = message.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.Repository.Name, resource.Resource.PullRequestId, "pull request"));

        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о комментарии в PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task PullRequestCommentNotify(PullRequestCommentedPayload resource)
    {
        HashSet<string> users = resource.Resource.pullRequest.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
        string author = resource.Resource.comment.author.uniqueName;

        users.RemoveWhere(x => x.Contains(author.Substring(0, author.IndexOf('@'))));
        List<long> chatIds = GetChatIds(users.ToList());

        string message = FormatMarkdownToTelegram(resource.Message.Text + Environment.NewLine
            + $"Project: {resource.Resource.pullRequest.Repository.Name}" + Environment.NewLine +
            $"Title: {resource.Resource.pullRequest.Title}" + Environment.NewLine +
            $"Description: {resource.Resource.pullRequest.Description}" + Environment.NewLine +
            $"{Environment.NewLine}{resource.Resource.comment.content}");

        message = message.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.pullRequest.Repository.Name, resource.Resource.pullRequest.PullRequestId, "pull request"));

        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о создании рабочего элемента.
    /// </summary>
    /// <param name = "resource" > Данные о событии</param>
    /// <param name = "message" > Сообщение.</ param >
    /// < returns ></ returns >
    private Task WorkItemCreatedNotify(WorkItemCreatedCustomPayload resource)
    {
        Match matchItemId = Regex.Match(resource.Message.Text, @"#(\d+)");

        if (matchItemId.Success)
        {
            string itemId = matchItemId.Groups[1].Value;

            HashSet<string> users = new HashSet<string>();
            users.Add(resource.Resource.Fields.SystemAssignedTo.UniqueName);

            List<long> chatIds = GetChatIds(users.ToList());

            string message = FormatMarkdownToTelegram($"{resource.Resource.Fields.SystemWorkItemType} created by {resource.Resource.Fields.SystemCreatedBy?.DisplayName}" + Environment.NewLine
                + $"Project: {resource.Resource.Fields.SystemTeamProject}" + Environment.NewLine +
                $"Title: {resource.Resource.Fields.SystemTitle}" + Environment.NewLine +
                $"State: {resource.Resource.Fields.SystemState}" + Environment.NewLine +
                $"Assigned to: {resource.Resource.Fields.SystemAssignedTo.DisplayName}" + Environment.NewLine);

            message = message.Replace($"{resource.Resource.Fields.SystemWorkItemType}", Utilites.WorkItemLinkConfigure(resource.Resource.Fields.SystemTeamProject, itemId, resource.Resource.Fields.SystemWorkItemType));

            if (chatIds.Count > 0)
            {
                _telegramBotClient.SendMessage(chatIds.First(), message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Отправь уведомление о создании рабочего элемента.
    /// </summary>
    /// <param name = "resource" > Данные о событии</param>
    /// <param name = "message" > Сообщение.</ param >
    /// < returns ></ returns >
    private Task WorkItemUpdatedNotify(WorkItemUpdatedCustomPayload resource)
    {
        if (resource.Resource.Fields.SystemAssignedTo == null && resource.Resource.Fields.MicrosoftVSTSCommonPriority == null)
        {
            return Task.CompletedTask;
        }

        if (resource.Resource.Revision.Fields.SystemAssignedTo.UniqueName.Equals(resource.Resource.Revision.Fields.SystemChangedBy.UniqueName))
        {
            return Task.CompletedTask;
        }

        Match matchItemId = Regex.Match(resource.Message.Text, @"#(\d+)");

        if (matchItemId.Success)
        {
            HashSet<string> users = new HashSet<string>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{resource.Resource.Revision.Fields.SystemWorkItemType} was changed");
            sb.AppendLine($"Project: {resource.Resource.Revision.Fields.SystemTeamProject}");
            sb.AppendLine($"Title: {resource.Resource.Revision.Fields.SystemTitle}");
            sb.AppendLine($"State: {resource.Resource.Revision.Fields.SystemState}");

            string messageText = FormatMarkdownToTelegram(sb.ToString());

            if (resource.Resource.Fields.SystemAssignedTo != null)
            {
                messageText = messageText + Environment.NewLine + ($"New Assigned to: {resource.Resource.Fields.SystemAssignedTo.NewValue.DisplayName}" + Environment.NewLine +
                $"~Old Assigned to: {resource.Resource.Fields.SystemAssignedTo.OldValue.DisplayName}~");

                users.Add(resource.Resource.Fields.SystemAssignedTo.NewValue.UniqueName);
            }
            if (resource.Resource.Fields.MicrosoftVSTSCommonPriority != null)
            {
                messageText = messageText + Environment.NewLine + ($"New Priority: {resource.Resource.Fields.MicrosoftVSTSCommonPriority.NewValue}" + Environment.NewLine +
                $"~Old Priority: {resource.Resource.Fields.MicrosoftVSTSCommonPriority.OldValue}~");

                if (!users.Any())
                {
                    users.Add(resource.Resource.Revision.Fields.SystemAssignedTo.UniqueName);
                }
            }

            string itemId = matchItemId.Groups[1].Value;

            messageText = messageText.Replace($"{resource.Resource.Revision.Fields.SystemWorkItemType}", Utilites.WorkItemLinkConfigure(resource.Resource.Revision.Fields.SystemTeamProject, itemId, resource.Resource.Revision.Fields.SystemWorkItemType));

            List<long> chatIds = GetChatIds(users.ToList());

            if (chatIds.Count > 0)
            {
                _telegramBotClient.SendMessage(chatIds.First(), messageText, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Оповещения о смене состояния сборки
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private Task BuildStateChangedNotify(BuildCompletedPayload resource)
    {
        HashSet<string> users = new HashSet<string>();

        users.Add(resource.Resource.LastChangedBy.UniqueName);
        List<long> chatIds = GetChatIds(users.ToList());

        if (chatIds.Count > 0)
        {
            _telegramBotClient.SendMessage(chatIds.First(), FormatMarkdownToTelegram(resource.Message.Text), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }

        return Task.CompletedTask;
    }

    private List<long> GetChatIds(List<string> displayNames)
    {
        var users = _appContext.Users.Where(b => displayNames.Any(pattern => EF.Functions.ILike(b.Login, "%" + pattern + "%"))).ToList();

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
        return Markdown.Escape(markdown);
    }
}


