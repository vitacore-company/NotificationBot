using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotificationsBot.Utils
{
    public static class Utilites
    {
        public static object GetUniqueUser(object user)
        {
            if (user is not string str)
            {
                str = user.ToString();
            }

            int startIndex = str.IndexOf("<") + 1;
            int endIndex = str.IndexOf(">");
            return str.Substring(startIndex, endIndex - startIndex);
        }

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
    }
}
