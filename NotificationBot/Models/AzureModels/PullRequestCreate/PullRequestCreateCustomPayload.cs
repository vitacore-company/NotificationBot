using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels
{
    public class PullRequestCreateCustom : GitPullRequestResource
    {
        [AllowNull]
        [JsonProperty("isDraft")]
        public bool IsDraft { get; set; }
    }

    public class PullRequestCreateCustomPayload : BasePayload<PullRequestCreateCustom>
    {
    }
}
