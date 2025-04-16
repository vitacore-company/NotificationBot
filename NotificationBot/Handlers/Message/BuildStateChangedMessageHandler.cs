using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.BuildStateChanged;
using NotificationsBot.Utils;
using System.Text;
using System.Text.RegularExpressions;
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
        public BuildStateChangedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger) : base(context, botClient, userHolder, logger)
        {
        }

        public async Task Handle(BuildStateChangedCustomPayload resource)
        {
            HashSet<string> users = new HashSet<string>();

            users.Add(resource.Resource.RequestedFor.UniqueName);
            List<long> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Project.Name, await _userHolder.GetChatIdsByLogin(users.ToList()));

            if (chatIds.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"Build {Utilites.BuildLinkConfigure(resource.Resource.BuildNumber, resource.Resource.Project.Name, resource.Resource.Id)} {resource.Resource.Result}");
                sb.AppendLine();
                sb.Append("*Project*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Project.Name));
                sb.AppendLine();
                sb.Append("*Definition*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Definition.Name));

                if (resource.Resource.Result.Equals("failed"))
                {
                    string messageText = Regex.Replace(resource.DetailedMessage.Text, @"^Build.*?failed\r\n\r\n- ", "", RegexOptions.Multiline);
                    if (!string.IsNullOrEmpty(messageText))
                    {
                        sb.AppendLine();
                        sb.Append($"```{FormatMarkdownToTelegram(messageText)}```");
                    }
                }

                sb.AppendLine();
                sb.AppendLine(FormatMarkdownToTelegram($"#{resource.Resource.Project.Name.Replace('.', '_').Replace("(agile)", "")} #Build"));

                string message = sb.ToString();

                _logger.LogInformation($"Состояние сборки изменено, сообщение отправлено {string.Join(',', chatIds)}");
                _ = _botClient.SendMessage(chatIds.First(), message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
    }
}
