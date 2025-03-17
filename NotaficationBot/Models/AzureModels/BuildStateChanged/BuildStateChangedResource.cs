using Newtonsoft.Json;

namespace NotificationsBot.Models.AzureModels.BuildStateChanged
{
    public class BuildStateChangedResource
    {
        [JsonProperty("requestedBy")]
        public RequestedBy requestedBy { get; set; }
    }
}
