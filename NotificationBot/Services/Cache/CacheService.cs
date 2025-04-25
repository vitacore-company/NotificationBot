using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using NotificationsBot.Interfaces;
using System.Collections.Concurrent;

namespace NotificationsBot.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly static ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();
        private readonly static ConcurrentDictionary<string, List<string>> _dependencies = new();

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Общий метод получения или кеширования данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <param name="dependencies"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, List<string>? dependencies = null, TimeSpan? absoluteExpiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T? cachedValue))
            {
                if (cachedValue != null)
                {
                    return cachedValue;
                }
            }

            T? value = await factory.Invoke();

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            if (absoluteExpiration.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromHours(5));
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            options.AddExpirationToken(new CancellationChangeToken(cts.Token));
            _tokens.TryAdd(key, cts);

            if (dependencies != null)
            {
                foreach (string dependency in dependencies)
                {
                    _dependencies.AddOrUpdate(dependency, new List<string> { key },
                        (_, list) =>
                        {
                            list.Add(key);

                            return list;
                        });
                }
            }

            _memoryCache.Set(key, value, options);
            return value;
        }

        /// <summary>
        /// Метод удаления значения из кеша по ключу
        /// </summary>
        /// <param name="key"></param>
        public void InvalidateByKey(string key)
        {
            if (_tokens.TryRemove(key, out CancellationTokenSource? cts))
            {
                cts.Cancel();
                cts.Dispose();
                _memoryCache.Remove(key);
            }
        }

        /// <summary>
        /// Метод удаления зависимостей кеша
        /// </summary>
        /// <param name="dependencies"></param>
        public void InvalidateDependencies(List<string> dependencies)
        {
            foreach (string dependency in dependencies)
            {
                if (_dependencies.TryRemove(dependency, out List<string>? keys))
                {
                    foreach (string key in keys)
                    {
                        InvalidateByKey(key);
                    }
                }
            }
        }
    }
}
