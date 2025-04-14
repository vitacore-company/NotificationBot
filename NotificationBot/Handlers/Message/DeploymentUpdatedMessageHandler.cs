using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.Release;
using NotificationsBot.Utils;
using System.Text;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    public class DeploymentUpdatedMessageHandler : BaseMessageHandler, IMessageHandler<Root>
    {
        public DeploymentUpdatedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger) : base(context, botClient, userHolder, logger)
        {
        }

        public async System.Threading.Tasks.Task Handle(Root resource)
        {
            string eventType = resource.resource.environment.releaseDefinition.path.Contains("Debug") ? "\\\\" : resource.resource.environment.releaseDefinition.path.Replace("\\", "\\\\");
            var notificationType = _context.NotificationTypes.Where(x => x.EventType == eventType).Select(x => x.Id).FirstOrDefault();

            if (notificationType > 0)
            {
                List<string> users = _context.NotificationsOnProjectChat.Where(x => x.NotificationTypesId == notificationType).Select(x => x.Users.Login).ToList();

                if (users.Count > 0)
                {
                    List<long> chatIds = await FilteredByNotifyUsers(
                        eventType,
                        resource.resource.project.name,
                        await _userHolder.GetChatIdsByLogin(users));

                    string link = Utilites.DeploymentLinkConfigure(
                        resource.resource.project.name,
                        resource.resource.environment.releaseId,
                        resource.resource.environment.id,
                        resource.resource.environment.name);

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"Deployment on stage {link} {resource.resource.deployment.deploymentStatus}");
                    sb.Append("*Release Definition*: ");
                    sb.Append(FormatMarkdownToTelegram(resource.resource.environment.releaseDefinition.name));
                    sb.AppendLine();
                    sb.Append("*Release Name*: ");
                    sb.Append(FormatMarkdownToTelegram(resource.resource.environment.release.name));
                    sb.AppendLine();
                    sb.Append("*Project*: ");
                    sb.Append(FormatMarkdownToTelegram(resource.resource.project.name));
                    sb.AppendLine();

                    foreach (ReleaseDeployPhase? item in resource.resource.environment.deploySteps.Select(x => x.releaseDeployPhases).FirstOrDefault())
                    {
                        foreach (DeploymentJob deployItem in item.deploymentJobs)
                        {
                            foreach (var task in deployItem.tasks)
                            {
                                if (task.status == item.status && task.issues.Count > 0)
                                {
                                    sb.Append("*Failed Task Name*: ");
                                    sb.Append(FormatMarkdownToTelegram(task.name));
                                    sb.AppendLine();
                                    sb.Append($"``` {FormatMarkdownToTelegram(task.issues[0].message)} ```");
                                }
                            }
                        }
                    }

                    string message = sb.ToString();

                    _logger.LogInformation($"Деплой запущен, сообщение отправлено {string.Join(',', chatIds)}");
                    foreach (long chatId in chatIds)
                    {
                        _ = _botClient.SendMessage(chatId, message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
                    }
                }
            }
        }
    }
}
