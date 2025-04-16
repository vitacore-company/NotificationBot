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
        public PullRequestCommentMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger) : base(context, botClient, userHolder, logger)
        {
        }

        public async Task Handle(PullRequestCommentedPayload resource)
        {
            HashSet<string> users = resource.Resource.pullRequest.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
            users.Add(resource.Resource.pullRequest.CreatedBy.UniqueName); // - добавляем автора пра отдельно

            string author = resource.Resource.comment.author.uniqueName;

            users.RemoveWhere(x => x.Contains(author.Substring(0, author.IndexOf('@'))));
            List<long> chatIds = await FilteredByNotifyUsers(
                resource.EventType,
                resource.Resource.pullRequest.Repository.Project.Name,
                await _userHolder.GetChatIdsByLogin(users.ToList()));

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
            sb.AppendLine($"`{FormatMarkdownToTelegram(resource.Resource.comment.content)}`");

            sb.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.pullRequest.Repository.Project.Name, resource.Resource.pullRequest.Repository.Name, resource.Resource.pullRequest.PullRequestId, "pull request"));

            sb.AppendLine();
            sb.AppendLine(FormatMarkdownToTelegram($"#{resource.Resource.pullRequest.Repository.Project.Name.Replace('.', '_').Replace("(agile)", "")} #PullRequestComment"));

            string message = sb.ToString();

            _logger.LogInformation($"Запрос на вытягивание {resource.Resource.pullRequest.PullRequestId} прокомментирован, сообщение отправлено {string.Join(',', chatIds)}");
            foreach (long chatId in chatIds)
            {
                _ =_botClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
    }
}
