using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;
public class PullRequestCommentedResource : BaseResource
{
    [AllowNull]
    [JsonProperty("pullRequest")]
    public GitPullRequestResource pullRequest { get; set; }
    
    [AllowNull]
    [JsonProperty("comment")]
    public Comment comment { get; set; }
}

public class PullRequestCommentedPayload : BasePayload<PullRequestCommentedResource>
{
}

