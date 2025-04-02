using Microsoft.AspNet.WebHooks.Payloads;
using Newtonsoft.Json;
using NotificationsBot.Models.AzureModels.WorkItemCreated;
using NotificationsBot.Models.AzureModels.WorkItemUpdated;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.WorkItemUpdated
{
    public class WorkItemUpdatedCustomResource : BaseResource
    {
        [AllowNull]
        [JsonProperty("fields")]
        public WorkItemUpdatedCustomFields Fields { get; set; }

        [AllowNull]
        [JsonProperty("revisedBy")]
        public ResourceUser RevisedBy { get; set; }

        [AllowNull]
        [JsonProperty("revision")]
        public Revision Revision { get; set; }
    }
}

public class WorkItemUpdatedCustomPayload : BasePayload<WorkItemUpdatedCustomResource>
{
}

public class WorkItemUpdatedCustomFields : WorkItemUpdatedFields
{
    [AllowNull]
    [JsonProperty("System.AssignedTo")]
    public new AssignedTo SystemAssignedTo { get; set; }
    
    [AllowNull]
    [JsonProperty("Microsoft.VSTS.Common.Priority")]
    public MicrosoftVSTSCommonPriority MicrosoftVSTSCommonPriority { get; set; }
}

public class AssignedTo
{
    [AllowNull]
    [JsonProperty("oldValue")]
    public ResourceUser OldValue { get; set; }

    [AllowNull]
    [JsonProperty("newValue")]
    public ResourceUser NewValue { get; set; }
}

public class MicrosoftVSTSCommonPriority
{
    [AllowNull]
    [JsonProperty("oldValue")]
    public string OldValue { get; set; }

    [AllowNull]
    [JsonProperty("newValue")]
    public string NewValue { get; set; }
}

public class Revision
{
    [AllowNull]
    [JsonProperty("fields")]
    public WorkItemCustomFields Fields { get; set; }
}