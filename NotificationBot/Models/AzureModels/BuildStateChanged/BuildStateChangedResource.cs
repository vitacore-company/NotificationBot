using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;

namespace NotificationsBot.Models.AzureModels.BuildStateChanged
{
    public class BuildStateChangedResource : BuildCompletedResource
    {
        [JsonProperty("requestedBy")]
        public ResourceUser RequestedBy { get; set; }

        [JsonProperty("project")]
        public GitProject Project { get; set; }
    }

    public class BuildStateChangedCustomPayload : BasePayload<BuildStateChangedResource>
    {

    }
}
