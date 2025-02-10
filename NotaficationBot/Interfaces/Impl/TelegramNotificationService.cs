using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using NotificationsBot.Models.AzureModels.PullRequestComment;
using NotificationsBot.Models.AzureModels.PullRequestCreated;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using User = NotificationsBot.Models.User;

namespace NotificationsBot.Interfaces.Impl;

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
    /// Уведомляет об указанном событии.
    /// </summary>
    /// <param name="eventNotification">Уведомление о событии.</param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/azure/devops/service-hooks/events?view=azure-devops">Типы событий</see>
    /// </remarks>
    public Task Notify(Event eventNotification)
    {
        string resultMessage = string.Empty;
        long chatid;
        switch (eventNotification.EventType)
        {
            case "git.push"://Code is pushed to a Git repository.
                break;
            case "git.pullrequest.created"://A pull request is created in a Git repository.
                //string testJson = "{\"repository\":{\"id\":\"dbebb6c7-1d1d-4da9-bbf1-7f266b077951\",\"name\":\"Smev-Adapter\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951\",\"project\":{\"id\":\"910a09ee-65b8-49ba-9797-c49514f44145\",\"name\":\"Smev-Adapter\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/_apis/projects/910a09ee-65b8-49ba-9797-c49514f44145\",\"state\":\"wellFormed\",\"revision\":823258,\"visibility\":\"private\",\"lastUpdateTime\":\"2024-03-07T07:21:46.503Z\"},\"size\":8166803,\"remoteUrl\":\"https://tfs.dev.vitacore.ru/tfs/Smev-Adapter/_git/Smev-Adapter\",\"sshUrl\":\"ssh://tfs.dev.vitacore.ru:22/tfs/Smev-Adapter/_git/Smev-Adapter\",\"webUrl\":\"https://tfs.dev.vitacore.ru/tfs/Smev-Adapter/_git/Smev-Adapter\",\"isDisabled\":false,\"isInMaintenance\":false},\"pullRequestId\":5489,\"codeReviewId\":5489,\"status\":\"active\",\"createdBy\":{\"displayName\":\"Sergej E. Nazarov\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/_apis/Identities/57da0ef2-ebd1-47e4-9e9d-b4fe9678aa64\",\"_links\":{\"avatar\":{\"href\":\"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMzc4OA\"}},\"id\":\"57da0ef2-ebd1-47e4-9e9d-b4fe9678aa64\",\"uniqueName\":\"DEV\\\\Sergej.Nazarov\",\"imageUrl\":\"https://tfs.dev.vitacore.ru/tfs/_api/_common/identityImage?id=57da0ef2-ebd1-47e4-9e9d-b4fe9678aa64\",\"descriptor\":\"win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMzc4OA\"},\"creationDate\":\"2025-02-06T10:52:02.6314911Z\",\"title\":\"тест телеграм бота\",\"description\":\"добавлена конфигурация для запуска spa\",\"sourceRefName\":\"refs/heads/feature/9638\",\"targetRefName\":\"refs/heads/develop\",\"mergeStatus\":\"conflicts\",\"isDraft\":false,\"mergeId\":\"deeb51ef-c9b3-43e6-8c8f-bd9febcd53a1\",\"lastMergeSourceCommit\":{\"commitId\":\"c71788d7c09fe0f4f8e1f47bc14e8a321bce9213\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951/commits/c71788d7c09fe0f4f8e1f47bc14e8a321bce9213\"},\"lastMergeTargetCommit\":{\"commitId\":\"5ec8377cc6b6229dbf0fbf8d9251e96d87bc91cb\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951/commits/5ec8377cc6b6229dbf0fbf8d9251e96d87bc91cb\"},\"reviewers\":[{\"reviewerUrl\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951/pullRequests/5489/reviewers/a980e8c8-c221-4397-8510-22d7c8bb9d8c\",\"vote\":0,\"hasDeclined\":false,\"isFlagged\":false,\"displayName\":\"Timur R. Galiullin\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/_apis/Identities/a980e8c8-c221-4397-8510-22d7c8bb9d8c\",\"_links\":{\"avatar\":{\"href\":\"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMzc5Mw\"}},\"id\":\"a980e8c8-c221-4397-8510-22d7c8bb9d8c\",\"uniqueName\":\"DEV\\\\Timur.Galiullin\",\"imageUrl\":\"https://tfs.dev.vitacore.ru/tfs/_api/_common/identityImage?id=a980e8c8-c221-4397-8510-22d7c8bb9d8c\"},{\"reviewerUrl\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951/pullRequests/5489/reviewers/0774eae3-02e4-4d8f-acbf-e26f54af1e29\",\"vote\":0,\"hasDeclined\":false,\"isFlagged\":false,\"displayName\":\"Dmitry E. Plaksin\",\"url\":\"https://tfs.dev.vitacore.ru/tfs/_apis/Identities/0774eae3-02e4-4d8f-acbf-e26f54af1e29\",\"_links\":{\"avatar\":{\"href\":\"https://tfs.dev.vitacore.ru/tfs/_apis/GraphProfile/MemberAvatars/win.Uy0xLTUtMjEtMjA5MzczODYzNy0yNjk5NzIxNTYyLTEzMDY0MzcyNDctMTE1Mw\"}},\"id\":\"0774eae3-02e4-4d8f-acbf-e26f54af1e29\",\"uniqueName\":\"DEV\\\\Dmitry.Plaksin\",\"imageUrl\":\"https://tfs.dev.vitacore.ru/tfs/_api/_common/identityImage?id=0774eae3-02e4-4d8f-acbf-e26f54af1e29\"}],\"url\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951/pullRequests/5489\",\"_links\":{\"web\":{\"href\":\"https://tfs.dev.vitacore.ru/tfs/Smev-Adapter/_git/Smev-Adapter/pullrequest/5489\"},\"statuses\":{\"href\":\"https://tfs.dev.vitacore.ru/tfs/910a09ee-65b8-49ba-9797-c49514f44145/_apis/git/repositories/dbebb6c7-1d1d-4da9-bbf1-7f266b077951/pullRequests/5489/statuses\"}},\"supportsIterations\":true,\"artifactId\":\"vstfs:///Git/PullRequestId/910a09ee-65b8-49ba-9797-c49514f44145%2fdbebb6c7-1d1d-4da9-bbf1-7f266b077951%2f5489\"}";
                PullRequestCreatedResource resource =((JsonElement)eventNotification.Resource).Deserialize<PullRequestCreatedResource>();
                PullRequestCreatedNotify(resource, eventNotification.DetailedMessage.Markdown);
                break;
            case "git.pullrequest.merge.attempted"://A pull request merge is attempted in a Git repository.
                break;
            case "git.pullrequest.approved":// A merge commit is approved on a pull request.
                break;
            case "git.pullrequest.updated"://Pull request updated
                PullRequestCreatedNotify(((JsonElement)eventNotification.Resource).Deserialize<PullRequestCreatedResource>(), eventNotification.DetailedMessage.Markdown);
                break;
            case "ms.vss-code.git-pullrequest-comment-event"://Pull request commented on
                PullRequestCommentNotify(((JsonElement)eventNotification.Resource).Deserialize<PullRequestCommentedResource>(), eventNotification.DetailedMessage.Markdown);
                break;
            case "workitem.created"://Work item created
                break;
            case "workitem.deleted"://Work item deleted
                break;
            case "workitem.restored"://A work item is newly restored.
                break;
            case "workitem.updated"://A work item is changed.
                break;
            case "workitem.commented":// A work item is commented on.
                break;
        }
        return Task.CompletedTask;
        //throw new NotImplementedException();
    }

    private Task PullRequestCreatedNotify(PullRequestCreatedResource resource, string message)
    {
        HashSet<string> users = resource.reviewers.Select(reviewer => reviewer.uniqueName)?.ToHashSet() ?? new HashSet<string>();
        users.Add(resource.createdBy.uniqueName);
        List<long> chatIds = GetChatIds(users.ToList());
        foreach (var chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, FormatMarkdownToTelegram(message), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
        return Task.CompletedTask;
    }
    
    private Task PullRequestCommentNotify(PullRequestCommentedResource resource, string message)
    {
        HashSet<string> users = resource.pullRequest.reviewers.Select(reviewer => reviewer.uniqueName)?.ToHashSet() ?? new HashSet<string>();
        users.Add(resource.comment.author.uniqueName);
        users.Add(resource.pullRequest.createdBy.uniqueName);
        List<long> chatIds = GetChatIds(users.ToList());
        foreach (var chatId in chatIds)
        {
            _telegramBotClient.SendMessage(chatId, FormatMarkdownToTelegram(message), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }
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
        List<User> users = _appContext.Users.Where(user => displayNames.Contains(user.Login)).ToList();
        if (users.Count == 0)
            return new List<long>();
        return users.Select(user => user.ChatId).ToList();
    }

    private string FormatMarkdownToTelegram(string markdown)
    {
        return Regex.Replace(markdown, "([\\\\_*`|!.[\\](){}>+#=~-])", "\\$1");
    }
}


