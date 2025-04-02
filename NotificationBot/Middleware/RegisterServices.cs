using NotificationsBot.Interfaces;
using System.Reflection;

namespace NotificationsBot.Middleware
{
    public static class RegisterServices
    {
        public static void AddMessageHandlers(this IServiceCollection services, Assembly assembly)
        {
            var messageHandlerType = typeof(IMessageHandler<>);

            // Получаем все типы в сборке, которые реализуют IMessageHandler<T>
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == messageHandlerType)
                .ToList();

            foreach (var handler in handlerTypes)
            {
                // Регистрируем реализацию в контейнере
                services.AddScoped(handler.Type, handler.Type);
            }
        }
    }
}
