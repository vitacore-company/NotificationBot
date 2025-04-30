using NotificationsBot.Extensions;
using NotificationsBot.Interfaces;
using NotificationsBot.Utils;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    /// <summary>
    /// Уведомление о создании рабочего элемента
    /// <remarks>
    /// workitem.created
    /// </remarks>
    /// </summary>
    public class WorkItemCreatedMessageHandler : BaseMessageHandler, IMessageHandler<WorkItemCreatedCustomPayload>
    {
        public WorkItemCreatedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger, ICacheService cacheService) : base(context, botClient, userHolder, logger, cacheService)
        {
        }

        public async Task Handle(WorkItemCreatedCustomPayload resource)
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

                Dictionary<long, int?> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Fields.SystemTeamProject, await _userHolder.GetChatIdsByLogin(users.ToList()), string.Empty);

                StringBuilder sb = new StringBuilder();
                sb.AddMainInfo(FormatMarkdownToTelegram($"{resource.Resource.Fields.SystemWorkItemType} created by {resource.Resource.Fields.SystemCreatedBy?.DisplayName}"));
                sb.AddProject(FormatMarkdownToTelegram(resource.Resource.Fields.SystemTeamProject));
                sb.AddTitle(FormatMarkdownToTelegram(resource.Resource.Fields.SystemTitle));
                sb.AppendLine();
                sb.Append("*State*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemState));
                sb.AppendLine();
                sb.Append("*Priority*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.MicrosoftVSTSCommonPriority));
                sb.AppendLine();
                if (!string.IsNullOrEmpty(GetTextFromHtml(resource.Resource.Fields.SystemDescription)))
                {
                    sb.Append("*Description*: ");
                    sb.Append(FormatMarkdownToTelegram(GetTextFromHtml(resource.Resource.Fields.SystemDescription)));
                    sb.AppendLine();
                }
                sb.Append("*Assigned to*: ");
                sb.AppendLine(FormatMarkdownToTelegram(resource.Resource.Fields.SystemAssignedTo.DisplayName));
                sb.AppendLine();

                sb.Replace($"{resource.Resource.Fields.SystemWorkItemType}", Utilites.WorkItemLinkConfigure(resource.Resource.Fields.SystemTeamProject, itemId, resource.Resource.Fields.SystemWorkItemType));

                sb.AppendLine();
                sb.AppendLine(FormatMarkdownToTelegram($"#{resource.Resource.Fields.SystemTeamProject.Replace('.', '_').Replace("(agile)", "")} #WorkItemCreate"));

                _logger.LogInformation($"Рабочий элемент {matchItemId} создан, сообщение отправлено {string.Join(',', chatIds)}");

                SendMessages(sb, chatIds);
            }
        }
    }
}
