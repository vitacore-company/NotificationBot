using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class ThreadsHref
{
    [AllowNull]
    public string href { get; set; }
}