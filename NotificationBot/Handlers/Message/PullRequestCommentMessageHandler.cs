using Microsoft.Extensions.Caching.Memory;
using NotificationsBot.Extensions;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.PullRequestComment;
using NotificationsBot.Utils;
using System.Text;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    /// <summary>
    /// Уведомление о комментировании пуллреквеста
    /// <remarks>
    /// ms.vss-code.git-pullrequest-comment-event
    /// </remarks>
    /// </summary>
    public class PullRequestCommentMessageHandler : BaseMessageHandler, IMessageHandler<PullRequestCommentedPayload>
    {
        public PullRequestCommentMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger, IMemoryCache memoryCache, INotificationCacheService notificationCache) : base(context, botClient, userHolder, logger, memoryCache, notificationCache)
        {
        }

        public async Task Handle(PullRequestCommentedPayload resource)
        {
            HashSet<string> users = resource.Resource.pullRequest.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
            users.Add(resource.Resource.pullRequest.CreatedBy.UniqueName); // - добавляем автора пра отдельно

            string author = resource.Resource.comment.author.uniqueName;

            users.RemoveWhere(x => x.Contains(author.Substring(0, author.IndexOf('@'))));
            Dictionary<long, int?> chatIds = await FilteredByNotifyUsers(
                resource.EventType,
                resource.Resource.pullRequest.Repository.Project.Name,
                await _userHolder.GetChatIdsByLogin(users.ToList()));

            StringBuilder sb = new StringBuilder();

            sb.AddMainInfo($"{FormatMarkdownToTelegram(resource.Resource.comment.author.displayName)} {GetLinkFromMarkdown(resource.DetailedMessage.Markdown)} pull request");
            sb.AddProject(Utilites.ProjectLinkConfigure(resource.Resource.pullRequest.Repository.Project.Name, resource.Resource.pullRequest.Repository.Name));
            sb.AddTitle(FormatMarkdownToTelegram(resource.Resource.pullRequest.Title));

            sb.AddDescription(FormatMarkdownToTelegram(resource.Resource.pullRequest.Description));
            sb.AppendLine();
            sb.AppendLine($"`{FormatMarkdownToTelegram(resource.Resource.comment.content)}`");

            sb.AddTags(resource.Resource.pullRequest.Repository.Project.Name, "PullRequestComment");

            string message = sb.ToString();

            _logger.LogInformation($"Запрос на вытягивание {resource.Resource.pullRequest.PullRequestId} прокомментирован, сообщение отправлено {string.Join(',', chatIds)}");

            SendMessages(sb, chatIds);
        }
    }
}
