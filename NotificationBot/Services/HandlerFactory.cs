using Newtonsoft.Json;
using NotificationsBot.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;

namespace NotificationsBot.Services
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HandlerFactory> _logger;
        public static readonly ConcurrentDictionary<Type, MethodInfo> _handleMethodCache = new();

        public HandlerFactory(IServiceProvider serviceProvider, ILogger<HandlerFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

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
                        throw new HandlerFactoryException($"Не удалось десереализовать объект с типом {handlerType.Name}");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"Ошибка десереализации объекта с типом {handlerType.Name}");
                    throw;
                }

                // Получение из кеша или кеширование обработчика
                if (!_handleMethodCache.TryGetValue(handlerType, out MethodInfo? handleMethod))
                {
                    handleMethod = handlerType.GetMethod(nameof(IMessageHandler<object>.Handle));

                    if (handleMethod == null)
                    {
                        throw new HandlerFactoryException("Не удалось получить обработчик!");
                    }

                    _handleMethodCache.TryAdd(handlerType, handleMethod);
                }

                // Вызов обработчика
                try
                {
                    Task? task = (Task?)handleMethod.Invoke(handler, new[] { payload });
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
