using NotificationsBot.Models.AzureModels.PullRequestCreated;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;
//todo ненужная хуйня,Возможно, нужно проверить
public class PullRequestCommentedResource
{
    public PullRequestCreatedResource pullRequest { get; set; }
    public Comment comment { get; set; }
}
