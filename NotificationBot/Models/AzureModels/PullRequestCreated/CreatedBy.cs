namespace NotificationsBot.Models.AzureModels.PullRequestCreated;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class CreatedBy
{
    public string id { get; set; }
    public string displayName { get; set; }
    public string uniqueName { get; set; }
    public string url { get; set; }
    public string imageUrl { get; set; }
}


