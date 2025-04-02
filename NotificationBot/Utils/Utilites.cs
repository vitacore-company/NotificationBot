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
            T? responseObject = JsonSerializer.Deserialize<T>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            if (responseObject != null)
            {
                return responseObject;
            }
            throw new ArgumentException("Данные не удалось преобразовать в объект");
        }

        public static string PullRequestLinkConfigure(string project, string repoName, int pullrequestId, string linkLabel)
        {
            string configLink = Markdown.Escape($"https://tfs.dev.vitacore.ru/tfs/{project}/_git/{repoName}/pullrequest/{pullrequestId}");
            string label = Markdown.Escape(linkLabel);

            string link = $"[{label}]({configLink})";

            return link;
        }

        public static string WorkItemLinkConfigure(string project, string itemId, string linkLabel)
        {
            string configLink = Markdown.Escape($"https://tfs.dev.vitacore.ru/tfs/{project}/_workitems/edit/{itemId}");
            string label = Markdown.Escape(linkLabel);

            string link = $"[{label}]({configLink})";

            return link;
        }

        public static string ProjectLinkConfigure(string project, string repoName)
        {
            string configLink = Markdown.Escape($"https://tfs.dev.vitacore.ru/tfs/{project}/_git/{repoName}");
            string repository = Markdown.Escape(repoName);

            string link = $"[{repository}]({configLink})";

            return link;
        }
    }
}
