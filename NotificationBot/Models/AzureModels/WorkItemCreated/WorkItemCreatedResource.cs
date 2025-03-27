using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using NotificationsBot.Models.AzureModels.WorkItemCreated;

namespace NotificationsBot.Models.AzureModels.WorkItemCreated
{
    public class WorkItemCreatedCustomResource : BaseResource
    {
        [JsonProperty("fields")]
        public WorkItemCustomFields Fields { get; set; }
    }
}

public class WorkItemCreatedCustomPayload : BasePayload<WorkItemCreatedCustomResource>
{
}

public class WorkItemCustomFields : WorkItemFields
{
    [JsonProperty("System.AssignedTo")]
    public ResourceUser SystemAssignedTo { get; set; }

    [JsonProperty("Microsoft.VSTS.TCM.ReproSteps")]
    public string MicrosoftVSTSTCMReproSteps { get; set; }

    [JsonProperty("System.Description")]
    public string SystemDescription { get; set; }

    [JsonProperty("System.CreatedBy")]
    public new ResourceUser SystemCreatedBy { get; set; }

    [JsonProperty("System.ChangedBy")]
    public new ResourceUser SystemChangedBy { get; set; }

    [JsonProperty("Microsoft.VSTS.Common.Priority")]
    public string MicrosoftVSTSCommonPriority { get; set; }
}