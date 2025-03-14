using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NotificationsBot.Models.AzureModels.WorkItemCreated
{
    public class WorkItemCreatedResource
    {
        public int Id { get; set; }
        public int Rev { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [JsonConverter(typeof(FieldDictionaryConverter))]
        public IDictionary<string, object> Fields { get; set; }
        public string Url { get; set; }
    }
}
