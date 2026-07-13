using NotificationsBot.Interfaces;

namespace NotificationsBot.Services
{
    public class ExistUserChecker : IExistUserChecker
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public ExistUserChecker(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            _client = client;
        }

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
#if DEBUG
            bool checkEnabled = _configuration.GetValue<bool>("EnableExistUserCheck");
            if (!checkEnabled)
            {
                return true;
            }
#endif
            CheckerUser openapiClient = new CheckerUser("http://192.168.20.127:9898", _client);
            return await openapiClient.GetAsync(userId);
        }
    }
}