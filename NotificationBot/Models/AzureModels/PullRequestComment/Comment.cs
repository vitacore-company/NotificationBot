using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class Comment
{
    public int id { get; set; }
    public int parentCommentId { get; set; }
    [AllowNull]
    public Author author { get; set; }
    [AllowNull]
    public string content { get; set; }
    public DateTime publishedDate { get; set; }
    public DateTime lastUpdatedDate { get; set; }
    public DateTime lastContentUpdatedDate { get; set; }
    [AllowNull]
    public string commentType { get; set; }
    [AllowNull]
    public Links _links { get; set; }
}
