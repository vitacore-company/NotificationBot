using Microsoft.AspNet.WebHooks.Payloads;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.BuildStateChanged;
using NotificationsBot.Models.AzureModels.PullRequestComment;
using NotificationsBot.Utils;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions;

namespace NotificationsBot.Services;

/// <summary>
/// Сервис уведомления для телеграмм бота
/// </summary>
/// <seealso cref="INotificationService" />
public class TelegramNotificationService : INotificationService
{
    private readonly AppContext _appContext;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUserHolder _userHolderService;

    public TelegramNotificationService(AppContext appContext, ITelegramBotClient telegramBotClient, IUserHolder userHolderService)
    {
        _appContext = appContext;
        _telegramBotClient = telegramBotClient;
        _userHolderService = userHolderService;
    }

    public async Task Notify(JsonElement element, string eventType)
    {
        switch (eventType)
        {
            case "git.pullrequest.updated":
                {
                    GitPullRequestUpdatedPayload? updated = JsonConvert.DeserializeObject<GitPullRequestUpdatedPayload>(element.ToString());

                    await PullRequestUpdatedNotify(updated);
                }
                break;

            case "ms.vss-code.git-pullrequest-comment-event":
                {
                    PullRequestCommentedPayload? pullRequestCommented = JsonConvert.DeserializeObject<PullRequestCommentedPayload>(element.ToString());

                    await PullRequestCommentNotify(pullRequestCommented);
                }
                break;

            case "git.pullrequest.created":
                {
                    GitPullRequestCreatedPayload? created = JsonConvert.DeserializeObject<GitPullRequestCreatedPayload>(element.ToString());

                    await PullRequestCreatedNotify(created);
                }
                break;

            case "workitem.created":
                {
                    string json = "{\r\n  \"subscriptionId\": \"a88c96ff-2faf-468b-90ae-4be73116b164\",\r\n  \"notificationId\": 25,\r\n  \"id\": \"ccf87ec5-2652-4739-8d2d-f58ef39d5995\",\r\n  \"eventType\": \"workitem.created\",\r\n  \"publisherId\": \"tfs\",\r\n  \"message\": {\r\n    \"text\": \"Task #11448 (Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения) created by Alexandr E. Plaksin\\r\\n(https://tfs.dev.vitacore.ru/web/wi.aspx?pcguid=14ed7439-51c3-4522-8939-cc6e31156c6a&id=11448)\",\r\n    \"html\": \"<a href=\\\"https://tfs.dev.vitacore.ru/web/wi.aspx?pcguid=14ed7439-51c3-4522-8939-cc6e31156c6a&amp;id=11448\\\">Task #11448</a> (Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения) created by Alexandr E. Plaksin\",\r\n    \"markdown\": \"[Task #11448](https://tfs.dev.vitacore.ru/web/wi.aspx?pcguid=14ed7439-51c3-4522-8939-cc6e31156c6a&id=11448) (Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения) created by Alexandr E. Plaksin\"\r\n  },\r\n  \"detailedMessage\": {\r\n    \"text\": \"Task #11448 (Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения) created by Alexandr E. Plaksin\\r\\n(https://tfs.dev.vitacore.ru/web/wi.aspx?pcguid=14ed7439-51c3-4522-8939-cc6e31156c6a&id=11448)\\r\\n\\r\\n- Area: ISZL(agile)\\r\\n- Iteration: ISZL(agile)\\\\Sprint 6\\r\\n- State: New\\r\\n\",\r\n    \"html\": \"<a href=\\\"https://tfs.dev.vitacore.ru/web/wi.aspx?pcguid=14ed7439-51c3-4522-8939-cc6e31156c6a&amp;id=11448\\\">Task #11448</a> (Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения) created by Alexandr E. Plaksin<ul>\\r\\n<li>Area: ISZL(agile)</li>\\r\\n<li>Iteration: ISZL(agile)\\\\Sprint 6</li>\\r\\n<li>State: New</li></ul>\",\r\n    \"markdown\": \"[Task #11448](https://tfs.dev.vitacore.ru/web/wi.aspx?pcguid=14ed7439-51c3-4522-8939-cc6e31156c6a&id=11448) (Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения) created by Alexandr E. Plaksin\\r\\n\\r\\n* Area: ISZL(agile)\\r\\n* Iteration: ISZL(agile)\\\\Sprint 6\\r\\n* State: New\\r\\n\"\r\n  },\r\n  \"resource\": {\r\n    \"id\": 11448,\r\n    \"rev\": 1,\r\n    \"fields\": {\r\n      \"System.AreaPath\": \"ISZL(agile)\",\r\n      \"System.TeamProject\": \"ISZL(agile)\",\r\n      \"System.IterationPath\": \"ISZL(agile)\\\\Sprint 6\",\r\n      \"System.WorkItemType\": \"Task\",\r\n      \"System.State\": \"New\",\r\n      \"System.Reason\": \"New\",\r\n      \"System.CreatedDate\": \"2025-03-28T13:30:12.533Z\",\r\n      \"System.CreatedBy\": {\r\n        \"displayName\": \"Alexandr E. Plaksin\",\r\n        \"url\": \"https://tfs.dev.vitacore.ru/tfs/_apis/Identities/13ddd6ca-0bf7-4d41-a47c-2007ab287253\",\r\n        \"_links\": {\r\n          \"avatar\": {\r\n            \"href\": \"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTM3MQ\"\r\n          }\r\n        },\r\n        \"id\": \"13ddd6ca-0bf7-4d41-a47c-2007ab287253\",\r\n        \"uniqueName\": \"DEV\\\\Alexandr.Plaksin\",\r\n        \"imageUrl\": \"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTM3MQ\",\r\n        \"descriptor\": \"win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTM3MQ\"\r\n      },\r\n      \"System.ChangedDate\": \"2025-03-28T13:30:12.533Z\",\r\n      \"System.ChangedBy\": {\r\n        \"displayName\": \"Alexandr E. Plaksin\",\r\n        \"url\": \"https://tfs.dev.vitacore.ru/tfs/_apis/Identities/13ddd6ca-0bf7-4d41-a47c-2007ab287253\",\r\n        \"_links\": {\r\n          \"avatar\": {\r\n            \"href\": \"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTM3MQ\"\r\n          }\r\n        },\r\n        \"id\": \"13ddd6ca-0bf7-4d41-a47c-2007ab287253\",\r\n        \"uniqueName\": \"DEV\\\\Alexandr.Plaksin\",\r\n        \"imageUrl\": \"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTM3MQ\",\r\n        \"descriptor\": \"win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTM3MQ\"\r\n      },\r\n      \"System.CommentCount\": 0,\r\n      \"System.Title\": \"Создать новый IScopedHandler для проверки прав пользователя при закрытии обращения\",\r\n      \"Microsoft.VSTS.Common.StateChangeDate\": \"2025-03-28T13:30:12.533Z\",\r\n      \"Microsoft.VSTS.Common.Priority\": 2,\r\n      \"System.Description\": \"<div>Зарегать в ISZL-овском&nbsp;ScopedSchemaStarter </div>\",\r\n      \"System.Parent\": 5441\r\n    },\r\n    \"_links\": {\r\n      \"self\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/workItems/11448\"\r\n      },\r\n      \"workItemUpdates\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/workItems/11448/updates\"\r\n      },\r\n      \"workItemRevisions\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/workItems/11448/revisions\"\r\n      },\r\n      \"workItemComments\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/workItems/11448/comments\"\r\n      },\r\n      \"html\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_workitems/edit/11448\"\r\n      },\r\n      \"workItemType\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/workItemTypes/Task\"\r\n      },\r\n      \"fields\": {\r\n        \"href\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/fields\"\r\n      }\r\n    },\r\n    \"url\": \"https://tfs.dev.vitacore.ru/tfs/543bf3ae-27f7-4f5f-a28c-c7e5ffc85890/_apis/wit/workItems/11448\"\r\n  },\r\n  \"resourceVersion\": \"5.1-preview.3\",\r\n  \"resourceContainers\": {\r\n    \"collection\": {\r\n      \"id\": \"14ed7439-51c3-4522-8939-cc6e31156c6a\",\r\n      \"baseUrl\": \"https://tfs.dev.vitacore.ru/tfs/\"\r\n    },\r\n    \"server\": {\r\n      \"id\": \"ba7421f6-fa42-4018-85b5-f7c14112d595\",\r\n      \"baseUrl\": \"https://tfs.dev.vitacore.ru/\"\r\n    },\r\n    \"project\": {\r\n      \"id\": \"543bf3ae-27f7-4f5f-a28c-c7e5ffc85890\",\r\n      \"baseUrl\": \"https://tfs.dev.vitacore.ru/tfs/\"\r\n    }\r\n  },\r\n  \"createdDate\": \"2025-03-28T13:30:20.2154196Z\"\r\n}\r\n";

                    WorkItemCreatedCustomPayload? created = JsonConvert.DeserializeObject<WorkItemCreatedCustomPayload>(json);

                    await WorkItemCreatedNotify(created);
                }
                break;

            case "workitem.updated":
                {
                    WorkItemUpdatedCustomPayload? updated = JsonConvert.DeserializeObject<WorkItemUpdatedCustomPayload>(element.ToString());

                    await WorkItemUpdatedNotify(updated);
                }
                break;

            case "build.complete":
                {
                    BuildStateChangedCustomPayload? build = JsonConvert.DeserializeObject<BuildStateChangedCustomPayload>(element.ToString());

                    await BuildStateChangedNotify(build);
                }
                break;
        }
    }

    /// <summary>
    /// Отправь уведомление о создании PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private async Task PullRequestCreatedNotify(GitPullRequestCreatedPayload resource)
    {
        HashSet<string> users = resource.Resource.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
        Match match = Regex.Match(resource.DetailedMessage.Text, @"\b\w+\s+\w\.\s+(\w+)");
        if (match.Success)
        {
            string remove = match.Groups[1].Value;
            users.RemoveWhere(x => x.Contains(remove));
        }

        List<long> chatIds = await _userHolderService.GetChatIdsByLogin(users.ToList());

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(FormatMarkdownToTelegram(resource.Message.Text.Substring(0, resource.Message.Text.LastIndexOf(resource.Resource.PullRequestId.ToString()))));
        sb.Append("*Project*: ");
        sb.Append(Utilites.ProjectLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name));
        sb.AppendLine();
        sb.Append("*Title*: ");
        sb.Append(FormatMarkdownToTelegram(resource.Resource.Title));
        sb.AppendLine();
        sb.Append("*Description*: ");
        sb.AppendLine(FormatMarkdownToTelegram(resource.Resource.Description));

        sb.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name, resource.Resource.PullRequestId, "pull request"));

        string message = sb.ToString();

        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
    }

    /// <summary>
    /// Отправь уведомление о комментарии в PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private async Task PullRequestUpdatedNotify(GitPullRequestUpdatedPayload resource)
    {
        if (resource.Message.Text.Contains("reviewer list"))
        {
            return;
        }

        HashSet<string> users = resource.Resource.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
        Match match = Regex.Match(resource.DetailedMessage.Text, @"\b\w+\s+\w\.\s+(\w+)");
        if (match.Success)
        {
            string remove = match.Groups[1].Value;
            users.RemoveWhere(x => x.Contains(remove));
        }

        List<long> chatIds = await _userHolderService.GetChatIdsByLogin(users.ToList());

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(FormatMarkdownToTelegram(resource.Message.Text.Substring(0, resource.Message.Text.LastIndexOf(resource.Resource.PullRequestId.ToString()))));
        sb.Append("*Project*: ");
        sb.Append(Utilites.ProjectLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name));
        sb.AppendLine();
        sb.Append("*Title*: ");
        sb.Append(FormatMarkdownToTelegram(resource.Resource.Title));
        sb.AppendLine();
        sb.Append("*Description*: ");
        sb.AppendLine(FormatMarkdownToTelegram(resource.Resource.Description));

        sb.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name, resource.Resource.PullRequestId, "pull request"));

        string message = sb.ToString();

        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
    }

    /// <summary>
    /// Отправь уведомление о комментарии в PR.
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private async Task PullRequestCommentNotify(PullRequestCommentedPayload resource)
    {
        HashSet<string> users = resource.Resource.pullRequest.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
        string author = resource.Resource.comment.author.uniqueName;

        users.RemoveWhere(x => x.Contains(author.Substring(0, author.IndexOf('@'))));
        List<long> chatIds = await _userHolderService.GetChatIdsByLogin(users.ToList());

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(FormatMarkdownToTelegram(resource.Message.Text));
        sb.Append("*Project*: ");
        sb.Append(Utilites.ProjectLinkConfigure(resource.Resource.pullRequest.Repository.Project.Name, resource.Resource.pullRequest.Repository.Name));
        sb.AppendLine();
        sb.Append("*Title*: ");
        sb.Append(FormatMarkdownToTelegram(resource.Resource.pullRequest.Title));
        sb.AppendLine();
        sb.Append("*Description*: ");
        sb.AppendLine(FormatMarkdownToTelegram(resource.Resource.pullRequest.Description));
        sb.AppendLine();
        sb.AppendLine(resource.Resource.comment.content);

        sb.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.pullRequest.Repository.Project.Name, resource.Resource.pullRequest.Repository.Name, resource.Resource.pullRequest.PullRequestId, "pull request"));

        string message = sb.ToString();

        foreach (long chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
    }

    /// <summary>
    /// Отправь уведомление о создании рабочего элемента.
    /// </summary>
    /// <param name = "resource" > Данные о событии</param>
    /// <param name = "message" > Сообщение.</ param >
    /// < returns ></ returns >
    private async Task WorkItemCreatedNotify(WorkItemCreatedCustomPayload resource)
    {
        if (resource.Resource.Fields.SystemAssignedTo == null)
        {
            return;
        }

        Match matchItemId = Regex.Match(resource.Message.Text, @"#(\d+)");

        if (matchItemId.Success)
        {
            string itemId = matchItemId.Groups[1].Value;

            HashSet<string> users = new HashSet<string>();
            users.Add(resource.Resource.Fields.SystemAssignedTo.UniqueName);

            List<long> chatIds = await _userHolderService.GetChatIdsByLogin(users.ToList());

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FormatMarkdownToTelegram($"{resource.Resource.Fields.SystemWorkItemType} created by {resource.Resource.Fields.SystemCreatedBy?.DisplayName}"));
            sb.Append("*Project*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemTeamProject));
            sb.AppendLine();
            sb.Append("*Title*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemTitle));
            sb.AppendLine();
            sb.Append("*State*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemState));
            sb.AppendLine();
            sb.Append("*Priority*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.MicrosoftVSTSCommonPriority));
            sb.AppendLine();
            sb.Append("*Assigned to*: ");
            sb.AppendLine(FormatMarkdownToTelegram(resource.Resource.Fields.SystemAssignedTo.DisplayName));
            sb.AppendLine();

            sb.Replace($"{resource.Resource.Fields.SystemWorkItemType}", Utilites.WorkItemLinkConfigure(resource.Resource.Fields.SystemTeamProject, itemId, resource.Resource.Fields.SystemWorkItemType));

            string message = sb.ToString();

            if (chatIds.Count > 0)
            {
                _telegramBotClient.SendMessage(chatIds.First(), message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
    }

    /// <summary>
    /// Отправь уведомление о создании рабочего элемента.
    /// </summary>
    /// <param name = "resource" > Данные о событии</param>
    /// <param name = "message" > Сообщение.</ param >
    /// < returns ></ returns >
    private async Task WorkItemUpdatedNotify(WorkItemUpdatedCustomPayload resource)
    {
        if (resource.Resource.Fields.SystemAssignedTo == null && resource.Resource.Fields.MicrosoftVSTSCommonPriority == null)
        {
            return;
        }

        if (resource.Resource.Revision.Fields.SystemAssignedTo.UniqueName.Equals(resource.Resource.Revision.Fields.SystemChangedBy.UniqueName))
        {
            return;
        }

        Match matchItemId = Regex.Match(resource.Message.Text, @"#(\d+)");

        if (matchItemId.Success)
        {
            HashSet<string> users = new HashSet<string>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FormatMarkdownToTelegram($"{resource.Resource.Revision.Fields.SystemWorkItemType} was changed"));
            sb.Append("*Project*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Revision.Fields.SystemTeamProject));
            sb.AppendLine();
            sb.Append("*Title*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Revision.Fields.SystemTitle));
            sb.AppendLine();
            sb.Append("*State*: ");
            sb.Append(FormatMarkdownToTelegram(resource.Resource.Revision.Fields.SystemState));
            sb.AppendLine();

            if (resource.Resource.Fields.SystemAssignedTo != null)
            {
                sb.Append("*New Assigned to*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemAssignedTo.NewValue.DisplayName));
                sb.AppendLine();
                sb.Append("~Old Assigned to: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemAssignedTo.OldValue.DisplayName) + "~");
                sb.AppendLine();

                users.Add(resource.Resource.Fields.SystemAssignedTo.NewValue.UniqueName);
            }
            if (resource.Resource.Fields.MicrosoftVSTSCommonPriority != null)
            {
                sb.Append("*New Priority*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.MicrosoftVSTSCommonPriority.NewValue));
                sb.AppendLine();
                sb.Append("~Old Priority: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.MicrosoftVSTSCommonPriority.OldValue) + "~");

                if (!users.Any())
                {
                    users.Add(resource.Resource.Revision.Fields.SystemAssignedTo.UniqueName);
                }
            }

            string itemId = matchItemId.Groups[1].Value;

            sb.Replace($"{resource.Resource.Revision.Fields.SystemWorkItemType}", Utilites.WorkItemLinkConfigure(resource.Resource.Revision.Fields.SystemTeamProject, itemId, resource.Resource.Revision.Fields.SystemWorkItemType));

            string messageText = sb.ToString();

            List<long> chatIds = await _userHolderService.GetChatIdsByLogin(users.ToList());

            if (chatIds.Count > 0)
            {
                _telegramBotClient.SendMessage(chatIds.First(), messageText, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
    }

    /// <summary>
    /// Оповещения о смене состояния сборки
    /// </summary>
    /// <param name="resource">Данные о событии</param>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private async Task BuildStateChangedNotify(BuildStateChangedCustomPayload resource)
    {
        HashSet<string> users = new HashSet<string>();

        users.Add(resource.Resource.RequestedBy.UniqueName);
        List<long> chatIds = await _userHolderService.GetChatIdsByLogin(users.ToList());

        if (chatIds.Count > 0)
        {
            _telegramBotClient.SendMessage(chatIds.First(), FormatMarkdownToTelegram(resource.Message.Text), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
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


