namespace NotificationsBot.Interfaces
{
    public interface IUserHolder
    {
        public Task<List<long>> GetChatIdsByLogin(List<string> login);

        public void Clear();
    }
}
