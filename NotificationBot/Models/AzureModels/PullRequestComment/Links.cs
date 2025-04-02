using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class Links
{
    [AllowNull]
    public SelfHref self { get; set; }
    [AllowNull]
    public RepositoryHref repository { get; set; }
    [AllowNull]
    public ThreadsHref threads { get; set; }
}
