using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NotificationsBot.Interfaces;
using System.Reflection;

namespace NotificationsBot.Services
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HandlerFactory> _logger;
        private readonly IMemoryCache _memoryCache;

        public HandlerFactory(IServiceProvider serviceProvider, ILogger<HandlerFactory> logger, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _memoryCache = memoryCache;

        }

        public async Task ProcessHandler(Type handlerType, string json)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                object? handler = scope.ServiceProvider.GetService(handlerType);
                if (handler == null)
                {
                    throw new InvalidOperationException($"Обработчик с типом {handlerType.Name} не зарегестрирован.");
                }

                // Получение интерфейса IMessageHandler<T>
                Type? messageHandlerInterface = handlerType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

                if (messageHandlerInterface == null)
                {
                    throw new InvalidOperationException($"Обработчик {handlerType.Name} не является наследником IMessageHandler<T>");
                }

                // Получение типа дженерика
                Type payloadType = messageHandlerInterface.GenericTypeArguments[0];

                // Попытка десериализовать в нужный тип из дженерика
                object? payload;
                try
                {
                    payload = JsonConvert.DeserializeObject(json, payloadType);
                    if (payload == null)
                    {
                        throw new HandlerFactoryException($"Не удалось десереализовать объект с типом {payloadType.Name}");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"Ошибка десереализации объекта с типом {payloadType.Name}");
                    throw;
                }

                // Получение из кеша или кеширование обработчика
                if (!_memoryCache.TryGetValue(handlerType, out MethodInfo? handleMethod))
                {
                    handleMethod = handlerType.GetMethod(nameof(IMessageHandler<object>.Handle));

                    if (handleMethod == null)
                    {
                        throw new HandlerFactoryException($"Не удалось получить обработчик с именем {handlerType.Name}!");
                    }

                    _memoryCache.Set(handlerType, handleMethod);
                }

                // Вызов обработчика
                try
                {
                    Task? task = (Task?)handleMethod?.Invoke(handler, new[] { payload });
                    if (task != null)
                    {
                        await task.ConfigureAwait(false);
                    }
                    else
                    {
                        throw new HandlerFactoryException();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка во время выполнения обработчика {handlerType.Name}");
                    throw;
                }
            }
        }
    }

    public class HandlerFactoryException : Exception
    {
        public HandlerFactoryException() { }
        public HandlerFactoryException(string message) : base(message) { }
        public HandlerFactoryException(string message, Exception inner) : base(message, inner) { }
    }
}
