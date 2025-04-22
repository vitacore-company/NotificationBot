using Microsoft.Extensions.Caching.Memory;
using NotificationsBot.Extensions;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.Release;
using NotificationsBot.Utils;
using System.Text;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    public class DeploymentUpdatedMessageHandler : BaseMessageHandler, IMessageHandler<Root>
    {
        public DeploymentUpdatedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger, IMemoryCache memoryCache) : base(context, botClient, userHolder, logger, memoryCache)
        {
        }

        public async System.Threading.Tasks.Task Handle(Root resource)
        {
            string eventType = resource.resource.environment.releaseDefinition.path.Contains("Regions") ? resource.resource.environment.releaseDefinition.path.Replace("\\", "\\\\") : "\\\\";

            int notificationType = _context.NotificationTypes.Where(x => x.EventType == eventType).Select(x => x.Id).FirstOrDefault();

            if (notificationType > 0)
            {
                List<string> users = _context.NotificationsOnProjectChat.Where(x => x.NotificationTypesId == notificationType).Select(x => x.Users.Login).ToList();

                if (users.Count > 0)
                {
                    Dictionary<long, int?> chatIds = await FilteredByNotifyUsers(
                        eventType,
                        resource.resource.project.name,
                        await _userHolder.GetChatIdsByLogin(users));

                    string link = Utilites.DeploymentLinkConfigure(
                        resource.resource.project.name,
                        resource.resource.environment.releaseId,
                        resource.resource.environment.id,
                        resource.resource.environment.name);

                    StringBuilder sb = new StringBuilder();

                    sb.AddMainInfo($"Deployment on stage {link} {resource.resource.deployment.deploymentStatus}");
                    sb.Append("*Release Definition*: ");
                    sb.Append(FormatMarkdownToTelegram(resource.resource.environment.releaseDefinition.name));
                    sb.AppendLine();
                    sb.Append("*Release Name*: ");
                    sb.Append(FormatMarkdownToTelegram(resource.resource.environment.release.name));
                    sb.AppendLine();
                    sb.AddProject(FormatMarkdownToTelegram(resource.resource.project.name));
                    sb.AppendLine();

                    List<ReleaseDeployPhase> deployPhaseList = resource.resource.environment.deploySteps.Select(x => x.releaseDeployPhases).FirstOrDefault() ?? new List<ReleaseDeployPhase>();
                    
                    foreach (ReleaseDeployPhase item in deployPhaseList)
                    {
                        foreach (DeploymentJob deployItem in item.deploymentJobs)
                        {
                            foreach (Models.AzureModels.Release.Task task in deployItem.tasks)
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

                    sb.AppendLine();
                    sb.AppendLine(FormatMarkdownToTelegram($"#{resource.resource.project.name.Replace('.', '_').Replace("(agile)", "")} #Deploy"));

                    _logger.LogInformation($"Деплой запущен, сообщение отправлено {string.Join(',', chatIds)}");

                    SendMessages(sb, chatIds);
                }
            }
        }
    }
}
