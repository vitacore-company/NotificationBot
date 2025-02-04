namespace NotificationsBot.Models.AzureModels.PullRequestComment;

public class Comment
{
    public int id { get; set; }
    public int parentCommentId { get; set; }
    public Author author { get; set; }
    public string content { get; set; }
    public DateTime publishedDate { get; set; }
    public DateTime lastUpdatedDate { get; set; }
    public DateTime lastContentUpdatedDate { get; set; }
    public string commentType { get; set; }
    public Links _links { get; set; }
}
