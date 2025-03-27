using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public static string PullRequestLinkConfigure(string project, int pullrequestId, string linkLabel)
        {
            string link = EscapeLink($"[{linkLabel}](https://tfs.dev.vitacore.ru/tfs/{project}/_git/{project}/pullrequest/{pullrequestId})");

            return link;
        }

        public static string WorkItemLinkConfigure(string project, string itemId, string linkLabel)
        {
            string link = EscapeLink($"[{linkLabel}](https://tfs.dev.vitacore.ru/tfs/{project}/_workitems/edit/{itemId})");

            return link;
        }

        public static string? EscapeLink(string? text)
        {
            if (text == null) return null;
            StringBuilder? sb = null;
            for (int index = 0, added = 0; index < text.Length; index++)
            {
                switch (text[index])
                {
                    case '_':
                    case '*':
                    case '~':
                    case '`':
                    case '#':
                    case '+':
                    case '-':
                    case '=':
                    case '.':
                    case '!':
                    case '{':
                    case '}':
                    case '>':
                    case '|':
                    case '\\':
                        sb ??= new StringBuilder(text, text.Length + 32);
                        sb.Insert(index + added++, '\\');
                        break;
                }
            }
            return sb?.ToString() ?? text;
        }
    }
}
