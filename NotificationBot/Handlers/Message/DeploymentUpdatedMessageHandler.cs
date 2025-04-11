using NotificationsBot.Interfaces;
using NotificationsBot.Models.AzureModels.Release;
using Telegram.Bot;

namespace NotificationsBot.Handlers
{
    public class DeploymentUpdatedMessageHandler : BaseMessageHandler, IMessageHandler<Root>
    {
        public DeploymentUpdatedMessageHandler(AppContext context, ITelegramBotClient botClient, IUserHolder userHolder, ILogger<BaseMessageHandler> logger) : base(context, botClient, userHolder, logger)
        {
        }

        public System.Threading.Tasks.Task Handle(Root resource)
        {
            throw new NotImplementedException();
        }
    }
}
