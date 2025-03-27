using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;

namespace NotificationsBot.Models.AzureModels.PullRequestComment;
public class PullRequestCommentedResource : BaseResource
{
    [JsonProperty("pullRequest")]
    public GitPullRequestResource pullRequest { get; set; }

    [JsonProperty("comment")]
    public Comment comment { get; set; }
}

public class PullRequestCommentedPayload : BasePayload<PullRequestCommentedResource>
{
}

