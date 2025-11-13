using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.BuildStateChanged
{
    public class BuildStateChangedResource : BuildCompletedResource
    {
        [AllowNull]
        [JsonProperty("requestedBy")]
        public ResourceUser RequestedBy { get; set; }
        
        [AllowNull]
        [JsonProperty("requestedFor")]
        public ResourceUser RequestedFor { get; set; }

        [AllowNull]
        [JsonProperty("project")]
        public GitProject Project { get; set; }

        [AllowNull]
        [JsonProperty("result")]
        public string Result { get; set; }
    }

    public class BuildStateChangedCustomPayload : BasePayload<BuildStateChangedResource>
    {

    }
}
