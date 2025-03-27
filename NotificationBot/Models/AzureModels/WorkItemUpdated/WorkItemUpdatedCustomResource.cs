using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using NotificationsBot.Models.AzureModels.WorkItemCreated;
using NotificationsBot.Models.AzureModels.WorkItemUpdated;

namespace NotificationsBot.Models.AzureModels.WorkItemUpdated
{
    public class WorkItemUpdatedCustomResource : BaseResource
    {
        [JsonProperty("fields")]
        public WorkItemUpdatedCustomFields Fields { get; set; }

        [JsonProperty("revisedBy")]
        public ResourceUser RevisedBy { get; set; }

        [JsonProperty("revision")]
        public Revision Revision { get; set; }
    }
}

public class WorkItemUpdatedCustomPayload : BasePayload<WorkItemUpdatedCustomResource>
{
}

public class WorkItemUpdatedCustomFields : WorkItemUpdatedFields
{
    [JsonProperty("System.AssignedTo")]
    public AssignedTo SystemAssignedTo { get; set; }

    [JsonProperty("Microsoft.VSTS.Common.Priority")]
    public new MicrosoftVSTSCommonPriority MicrosoftVSTSCommonPriority { get; set; }
}

public class AssignedTo
{
    [JsonProperty("oldValue")]
    public ResourceUser OldValue { get; set; }

    [JsonProperty("newValue")]
    public ResourceUser NewValue { get; set; }
}

public class MicrosoftVSTSCommonPriority
{
    [JsonProperty("oldValue")]
    public string OldValue { get; set; }

    [JsonProperty("newValue")]
    public string NewValue { get; set; }
}

public class Revision
{
    [JsonProperty("fields")]
    public WorkItemCustomFields Fields { get; set; }
}