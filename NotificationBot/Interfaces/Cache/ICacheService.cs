namespace NotificationsBot.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, List<string>? dependencies = null, TimeSpan? absoluteExpiration = null);
        void InvalidateByKey(string key);
        void InvalidateDependencies(List<string> dependencies);
    }
}
