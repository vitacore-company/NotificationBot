﻿using NotificationsBot.Extensions;
using NotificationsBot.Interfaces;
using NotificationsBot.Utils;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    /// <summary>
    /// Уведомление об изменении рабочего элемента
    /// <remarks>
    /// git.pullrequest.updated
    /// </remarks>
    /// </summary>
    public class WorkItemUpdatedMessageHandler : BaseMessageHandler, IMessageHandler<WorkItemUpdatedCustomPayload>
    {
        public WorkItemUpdatedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger, ICacheService cacheService) : base(context, botClient, userHolder, logger, cacheService)
        {
        }

        public async Task Handle(WorkItemUpdatedCustomPayload resource)
        {
            if (resource.Resource.Revision.Fields.SystemAssignedTo == null)
            {
                return;
            }
            if (resource.Resource.Fields == null || resource.Resource.Fields.SystemAssignedTo == null 
                && resource.Resource.Fields.MicrosoftVSTSCommonPriority == null
                && resource.Resource.Fields.Description == null)
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
                sb.AddMainInfo(FormatMarkdownToTelegram($"{resource.Resource.Revision.Fields.SystemWorkItemType} was changed"));
                sb.AddProject(FormatMarkdownToTelegram(resource.Resource.Revision.Fields.SystemTeamProject));
                sb.AddTitle(FormatMarkdownToTelegram(resource.Resource.Revision.Fields.SystemTitle));

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
                    sb.Append(FormatMarkdownToTelegram(resource.Resource.Fields.SystemAssignedTo.OldValue?.DisplayName ?? "") + "~");
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
                if (resource.Resource.Fields.Description != null)
                {
                    sb.Append("*Description*: ");
                    sb.Append(FormatMarkdownToTelegram(GetTextFromHtml(resource.Resource.Fields.Description.NewValue)));
                    sb.AppendLine();

                    if (!users.Any())
                    {
                        users.Add(resource.Resource.Revision.Fields.SystemAssignedTo.UniqueName);
                    }
                }

                string itemId = matchItemId.Groups[1].Value;

                sb.Replace($"{resource.Resource.Revision.Fields.SystemWorkItemType}", Utilites.WorkItemLinkConfigure(resource.Resource.Revision.Fields.SystemTeamProject, itemId, resource.Resource.Revision.Fields.SystemWorkItemType));

                sb.AddTags(resource.Resource.Revision.Fields.SystemTeamProject, "WorkItemUpdate");

                Dictionary<long, int?> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Revision.Fields.SystemTeamProject, await _userHolder.GetChatIdsByLogin(users.ToList()));

                _logger.LogInformation($"Рабочий элемент {matchItemId} измененен, сообщение отправлено {string.Join(',', chatIds)}");

                SendMessages(sb, chatIds);
            }
        }
    }
}
