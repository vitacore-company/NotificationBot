namespace NotificationsBot.Interfaces
{
    public interface IStartupTask
    {
        public Task ExecuteAsync(CancellationToken token = default);
    }
}
