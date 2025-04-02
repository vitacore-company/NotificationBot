﻿using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.BuildStateChanged;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    /// <summary>
    /// Уведомление о смене состояния сборки
    /// <remarks>
    /// build.complete
    /// </remarks>
    /// </summary>
    public class BuildStateChangedMessageHandler : BaseMessageHandler, IMessageHandler<BuildStateChangedCustomPayload>
    {
        public BuildStateChangedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder) : base(context, botClient, userHolder)
        {
        }

        public async Task Handle(BuildStateChangedCustomPayload resource)
        {
            HashSet<string> users = new HashSet<string>();

            users.Add(resource.Resource.RequestedFor.UniqueName);
            List<long> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Project.Name, await _userHolder.GetChatIdsByLogin(users.ToList()));

            if (chatIds.Count > 0)
            {
                _ = _botClient.SendMessage(chatIds.First(), FormatMarkdownToTelegram(resource.Message.Text), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
    }
}
