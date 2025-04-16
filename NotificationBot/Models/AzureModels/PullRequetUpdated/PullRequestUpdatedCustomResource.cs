using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.PullRequetUpdated
{
    public class PullRequestCustom : GitPullRequestUpdatedResource
    {
        [AllowNull]
        [JsonProperty("isDraft")]
        public bool IsDraft { get; set; }

        [AllowNull]
        [JsonProperty("lastMergeCommit")]
        public new GitMergeCommitCustom LastMergeCommit { get; set; }
    }

    public class PullRequestUpdatedCustomPayload : BasePayload<PullRequestCustom>
    {
    }

    public class GitMergeCommitCustom
    {
        [AllowNull]
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [AllowNull]
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [AllowNull]
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
