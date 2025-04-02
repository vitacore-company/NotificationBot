using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using NotificationsBot.Models.AzureModels.WorkItemCreated;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.WorkItemCreated
{
    public class WorkItemCreatedCustomResource : BaseResource
    {
        [AllowNull]
        [JsonProperty("fields")]
        public WorkItemCustomFields Fields { get; set; }
    }
}

public class WorkItemCreatedCustomPayload : BasePayload<WorkItemCreatedCustomResource>
{
}

public class WorkItemCustomFields : WorkItemFields
{
    [AllowNull]
    [JsonProperty("System.AssignedTo")]
    public ResourceUser SystemAssignedTo { get; set; }

    [AllowNull]
    [JsonProperty("Microsoft.VSTS.TCM.ReproSteps")]
    public string MicrosoftVSTSTCMReproSteps { get; set; }

    [AllowNull]
    [JsonProperty("System.Description")]
    public string SystemDescription { get; set; }

    [AllowNull]
    [JsonProperty("System.CreatedBy")]
    public new ResourceUser SystemCreatedBy { get; set; }

    [AllowNull]
    [JsonProperty("System.ChangedBy")]
    public new ResourceUser SystemChangedBy { get; set; }
    
    [AllowNull]
    [JsonProperty("Microsoft.VSTS.Common.Priority")]
    public string MicrosoftVSTSCommonPriority { get; set; }
}