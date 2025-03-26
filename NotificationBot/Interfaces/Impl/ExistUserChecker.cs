using NotificationsBot.Utils;

namespace NotificationsBot.Interfaces.Impl
{
    public class ExistUserChecker : IExistUserChecker
    {
        public async Task<bool> CheckExistUser(long userId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://192.168.20.127:9898/user/{userId}");
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return response.ToObject<bool>().Result;
                }
            }

            return false;
        }
    }
}
