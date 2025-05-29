using System.Net;

namespace NotificationsBot.Middleware
{
    public class DomainWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _allowedDomains;

        public DomainWhitelistMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _allowedDomains = configuration.GetSection("AllowedDomains").Get<List<string>>()
                ?? new List<string>();
        }

        /// <summary>
        /// Перехватывает каждый запрос и смотрит откуда он пришел
        /// </summary>
        /// <param name="context">Контекст запроса</param>
        /// <returns>Пропускаем, если пришел с адреса tfs.dev.vitacore.ru</returns>
        public async Task Invoke(HttpContext context)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry($"{context.Connection.RemoteIpAddress}");

            if (string.IsNullOrEmpty(hostEntry.HostName))
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
