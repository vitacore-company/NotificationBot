using Microsoft.AspNet.WebHooks.Payloads;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using NotificationsBot.Extensions;
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
                initiator = match.Value;

                users.RemoveWhere(x => x.Contains(remove));
            }

            if (resource.Message.Text.Contains("published the pull request"))
            {
                await redirectToPullRequestCreate(resource, initiator);
                return;
            }

            Regex regex = new Regex(@"^.*?(?=\bpull request\b)");
            Match matchText = regex.Match(resource.Message.Text);

            if (matchText.Success)
            {
                Dictionary<long, int?> chatIds = await FilteredByNotifyUsers(resource.EventType, resource.Resource.Repository.Project.Name, await _userHolder.GetChatIdsByLogin(users.ToList()));

                StringBuilder sb = new StringBuilder();

                sb.AddMainInfo($"{FormatMarkdownToTelegram(matchText.Value)} {GetLinkFromMarkdown(resource.DetailedMessage.Markdown)}");
                sb.AddProject(Utilites.ProjectLinkConfigure(resource.Resource.Repository.Project.Name, resource.Resource.Repository.Name));
                sb.AddTitle(FormatMarkdownToTelegram(resource.Resource.Title));

                sb.AddDescription(FormatMarkdownToTelegram(resource.Resource.Description));
                sb.AddTags(resource.Resource.Repository.Project.Name, "PullRequestUpdate");

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

            string markdownLink = Utilites.PullRequestLinkConfigure(payload.Resource.Repository.Project.Name, payload.Resource.Repository.Name, payload.Resource.PullRequestId, "pull request");

            createPayload.Resource.CreatedBy = new GitUser() { DisplayName = initiator };
            createPayload.Resource.Repository = payload.Resource.Repository;
            createPayload.EventType = "git.pullrequest.created";
            createPayload.DetailedMessage.Markdown = $"{initiator} create {markdownLink}";
            createPayload.DetailedMessage.Text = payload.DetailedMessage.Text;
            createPayload.Resource.Reviewers.AddRange(payload.Resource.Reviewers);
            createPayload.Resource.Title = payload.Resource.Title;
            createPayload.Resource.Description = payload.Resource.Description;

            await _createHandler.ProcessHandler(typeof(PullRequestCreateMessageHandler), JsonConvert.SerializeObject(createPayload));
        }
    }
}

