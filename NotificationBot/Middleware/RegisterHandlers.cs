using NotificationsBot.Handlers;
using NotificationsBot.Models.Locals;

namespace NotificationsBot.Middleware
{
    public static class RegisterHandlers
    {
        public static void RegisterHandler(this IServiceCollection service)
        {
            LocalMessageHandlers.Handlers.TryAdd("git.pullrequest.updated", typeof(PullRequestUpdateMessageHandler));
            LocalMessageHandlers.Handlers.TryAdd("ms.vss-code.git-pullrequest-comment-event", typeof(PullRequestCommentMessageHandler));
            LocalMessageHandlers.Handlers.TryAdd("git.pullrequest.created", typeof(PullRequestCreateMessageHandler));
            LocalMessageHandlers.Handlers.TryAdd("workitem.created", typeof(WorkItemCreatedMessageHandler));
            LocalMessageHandlers.Handlers.TryAdd("workitem.updated", typeof(WorkItemUpdatedMessageHandler));
            LocalMessageHandlers.Handlers.TryAdd("build.complete", typeof(BuildStateChangedMessageHandler));
        }
    }
}
