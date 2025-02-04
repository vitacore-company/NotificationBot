using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using NotificationsBot.Models.AzureModels.PullRequestComment;
using NotificationsBot.Models.AzureModels.PullRequestCreated;
using System.Text.Json;
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
                //JsonConvert.DeserializeObject<Resource>(eventNotification.Resource);
                PullRequestCreatedResource resource = ((JsonElement)eventNotification.Resource).Deserialize<PullRequestCreatedResource>();
                //chatid = GetChatId(resource.createdBy.uniqueName);
                PullRequestCreatedNotify(resource, eventNotification.Message.Markdown);
                //chatid = GetChatId(eventNotification.);
                break;
            case "git.pullrequest.merge.attempted"://A pull request merge is attempted in a Git repository.
                break;
            case "git.pullrequest.approved":// A merge commit is approved on a pull request.
                break;
            case "git.pullrequest.updated"://Pull request updated
                break;
            case "ms.vss-code.git-pullrequest-comment-event"://Pull request commented on
                PullRequestCommentNotify(((JsonElement)eventNotification.Resource).Deserialize<Comment>(), eventNotification.DetailedMessage.Markdown);
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
        long chatid = GetChatId(resource.createdBy.uniqueName);
        _telegramBotClient.SendMessage(chatid, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        //дописать уведомление ревьюеров
        return Task.CompletedTask;
    }
    
    private async Task PullRequestCommentNotify(Comment resource, string message)
    {
        long chatid = GetChatId(resource.author.uniqueName);
        var res = await _telegramBotClient.SendMessage(chatid, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);//не отправляет 
        
        return;
    }

    private long GetChatId(string displayName)
    {
        User? user = _appContext.Users.FirstOrDefault(user => user.Login == displayName);
        if (user == null)
            throw new NullReferenceException("Пользователь не найден");
        return user.ChatId;
    }
}


