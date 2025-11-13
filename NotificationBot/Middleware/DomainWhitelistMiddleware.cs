using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace NotificationsBot.Middleware
{
    public class DomainWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _allowedDomains;
        private readonly IMemoryCache _cacheService;

        public DomainWhitelistMiddleware(RequestDelegate next, IConfiguration configuration, IMemoryCache cacheService)
        {
            _next = next;
            _allowedDomains = configuration.GetSection("AllowedDomains").Get<List<string>>()
                ?? new List<string>();
            _cacheService = cacheService;
        }

        /// <summary>
        /// Перехватывает каждый запрос и смотрит откуда он пришел
        /// </summary>
        /// <param name="context">Контекст запроса</param>
        /// <returns>Пропускаем, если пришел с адреса tfs.dev.vitacore.ru</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Connection.RemoteIpAddress == null)
            {
                return;
            }

            if (!_cacheService.TryGetValue(context.Connection.RemoteIpAddress.ToString(), out IPHostEntry? hostEntry))
            {
                hostEntry = Dns.GetHostEntry($"{context.Connection.RemoteIpAddress}");
                _cacheService.Set(context.Connection.RemoteIpAddress.ToString(), hostEntry);
            }

            if (hostEntry == null || string.IsNullOrEmpty(hostEntry.HostName))
            {
                return;
            }

            bool isAllowed = _allowedDomains.Any(domain =>
                hostEntry.HostName.Contains(domain, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }

            await _next(context);
        }
    }
}
