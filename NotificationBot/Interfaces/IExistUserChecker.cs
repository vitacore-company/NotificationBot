namespace NotificationsBot.Interfaces
{
    public interface IExistUserChecker
    {
        public Task<bool> CheckExistUser(long userId);
    }
}
