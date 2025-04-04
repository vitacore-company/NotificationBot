using NotificationsBot.Interfaces;
using NotificationsBot.Utils;

namespace NotificationsBot.Services
{
    public class ExistUserChecker : IExistUserChecker
    {
        /// <summary>
        /// Проверяет существующего пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns></returns>
        public async Task<bool> CheckExistUser(long userId)
        {
            if (userId == -1) 
            {
                return false;
            }
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
