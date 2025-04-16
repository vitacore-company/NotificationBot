using Microsoft.AspNet.WebHooks.Payloads;
using NotificationsBot.Interfaces;
using NotificationsBot.Utils;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    /// <summary>
    /// Уведомление о создании пуллреквеста
    /// <remarks>
    /// git.pullrequest.created
    /// </remarks>
    /// </summary>
    public class PullRequestCreateMessageHandler : BaseMessageHandler, IMessageHandler<GitPullRequestCreatedPayload>
    {
        public PullRequestCreateMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger) : base(context, botClient, userHolder, logger)
        {
        }

        public async Task Handle(GitPullRequestCreatedPayload resource)
        {
            HashSet<string> users = resource.Resource.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
            Match match = Regex.Match(resource.DetailedMessage.Text, @"\b\w+\s+\w\.\s+(\w+)");
            if (match.Success)
            {
                string remove = match.Groups[1].Value;
                users.RemoveWhere(x => x.Contains(remove));
            }

            List<long> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Repository.Project.Name, await _userHolder.GetChatIdsByLogin(users.ToList()));

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

            sb.AppendLine();
            sb.AppendLine(FormatMarkdownToTelegram($"#{resource.Resource.Repository.Project.Name} #PullRequestCreate"));

            string message = sb.ToString();

            _logger.LogInformation($"Запрос на вытягивание {resource.Resource.PullRequestId} создан, сообщение отправлено {string.Join(',', chatIds)}");
            foreach (long chatId in chatIds)
            {
                _ = _botClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
    }
}
