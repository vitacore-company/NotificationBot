using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class RepositoryHref
{
    [AllowNull]
    public string href { get; set; }
}
