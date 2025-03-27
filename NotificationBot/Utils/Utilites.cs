using System.Text.Json;
using System.Text.Json.Serialization;
using Telegram.Bot.Extensions;

namespace NotificationsBot.Utils
{
    public static class Utilites
    {
        public static async Task<T> ToObject<T>(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            T responseObject = JsonSerializer.Deserialize<T>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            return responseObject;
        }

        public static string PullRequestLinkConfigure(string project, string repoName, int pullrequestId, string linkLabel)
        {
            string configLink = Markdown.Escape($"https://tfs.dev.vitacore.ru/tfs/{project}/_git/{repoName}/pullrequest/{pullrequestId}");

            string link = $"[{linkLabel}]({configLink})";

            return link;
        }

        public static string WorkItemLinkConfigure(string project, string itemId, string linkLabel)
        {
            string configLink = Markdown.Escape($"https://tfs.dev.vitacore.ru/tfs/{project}/_workitems/edit/{itemId}");

            string link = $"[{linkLabel}]({configLink})";

            return link;
        }
    }
}
