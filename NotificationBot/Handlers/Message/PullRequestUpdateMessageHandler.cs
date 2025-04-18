using Microsoft.AspNet.WebHooks.Payloads;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.PullRequetUpdated;
using NotificationsBot.Utils;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    /// <summary>
    /// Уведомление об обновлении пуллреквеста
    /// <remarks>
    /// git.pullrequest.updated
    /// </remarks>
    /// </summary>
    public class PullRequestUpdateMessageHandler : BaseMessageHandler, IMessageHandler<PullRequestUpdatedCustomPayload>
    {
        private readonly IHandlerFactory _createHandler;
        public PullRequestUpdateMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger, IHandlerFactory createHandler) : base(context, botClient, userHolder, logger)
        {
            _createHandler = createHandler;
        }

        public async Task Handle(PullRequestUpdatedCustomPayload resource)
        {
            if (resource.Resource.IsDraft)
            {
                return;
            }

            if (resource.Message.Text.Contains("reviewer list"))
            {
                return;
            }

            HashSet<string> users = resource.Resource.Reviewers.Select(reviewer => reviewer.UniqueName)?.ToHashSet() ?? new HashSet<string>();
            users.Add(resource.Resource.CreatedBy.UniqueName);

            Match match = Regex.Match(resource.DetailedMessage.Text, @"\b\w+\s+\w\.\s+(\w+)");
            string initiator = string.Empty;
            if (match.Success)
            {
                string remove = match.Groups[1].Value;

                if (users.Where(x => x.Contains(remove)).Count() > 0)
                {
                    initiator = users.Where(x => x.Contains(remove)).First();
                }

                users.RemoveWhere(x => x.Contains(remove));
            }

            Regex regex = new Regex(@"^.*?pull request");

            Match matchText = regex.Match(resource.Message.Text);
            if (matchText.Success)
            {
                string messageText = matchText.Value;

                if (messageText.Contains("published the pull request"))
                {
                    await redirectToPullRequestCreate(resource, match.Groups[1].Value);
                    return;
                }

                Dictionary<long, int?> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Repository.Project.Name, await _userHolder.GetChatIdsByLogin(users.ToList()));

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(FormatMarkdownToTelegram(messageText));
                sb.Append("*Project*: ");
                sb.Append(Utilites.ProjectLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name));
                sb.AppendLine();
                sb.Append("*Title*: ");
                sb.Append(FormatMarkdownToTelegram(resource.Resource.Title));
                sb.AppendLine();
                sb.Append("*Description*: ");
                sb.AppendLine();
                sb.AppendLine(FormatMarkdownToTelegram(resource.Resource.Description));

                sb.Replace("pull request", Utilites.PullRequestLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name, resource.Resource.PullRequestId, "pull request"));

                sb.AppendLine();
                sb.AppendLine(FormatMarkdownToTelegram($"#{resource.Resource.Repository.Project.Name.Replace('.', '_').Replace("(agile)", "")} #PullRequestUpdate"));

                _logger.LogInformation($"Запрос на вытягивание {resource.Resource.PullRequestId} измененен, сообщение отправлено {string.Join(',', chatIds)}");

                SendMessages(sb, chatIds);
            }
        }

        private async Task redirectToPullRequestCreate(PullRequestUpdatedCustomPayload payload, string initiator)
        {
            GitPullRequestCreatedPayload createPayload = new GitPullRequestCreatedPayload();

            createPayload.Resource = new GitPullRequestResource();
            createPayload.DetailedMessage = new PayloadMessage();
            createPayload.Message = new PayloadMessage();

            createPayload.Resource.Repository = payload.Resource.Repository;
            createPayload.EventType = "git.pullrequest.created";
            createPayload.DetailedMessage.Text = $"{initiator} create pull request";
            createPayload.Message.Text = $"{initiator} create pull request";
            createPayload.Resource.Reviewers.AddRange(payload.Resource.Reviewers);
            createPayload.Resource.Title = payload.Resource.Title;
            createPayload.Resource.Description = payload.Resource.Description;

            await _createHandler.ProcessHandler(typeof(PullRequestCreateMessageHandler), JsonConvert.SerializeObject(createPayload));
        }
    }
}

