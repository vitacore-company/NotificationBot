using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class SelfHref
{
    [AllowNull]
    public string href { get; set; }
}
