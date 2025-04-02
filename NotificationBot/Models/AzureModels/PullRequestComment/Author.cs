using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class Author
{
    [AllowNull]
    public string displayName { get; set; }
    [AllowNull]
    public string url { get; set; }
    [AllowNull]
    public string id { get; set; }
    [AllowNull]
    public string uniqueName { get; set; }
    [AllowNull]
    public string imageUrl { get; set; }
}
