using Newtonsoft.Json;
using NotificationsBot.Interfaces;

namespace NotificationsBot.Services
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HandlerFactory> _logger;

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

                // Получаем все интерфейсы, реализованные хендлером
                Type[] interfaces = handlerType.GetInterfaces();

                // Находим первый дженерик интерфейс
                Type? genericInterface = interfaces.FirstOrDefault(i => i.IsGenericType);

                if (genericInterface != null)
                {
                    // Получаем тип из GenericTypeArguments
                    Type payloadType = genericInterface.GenericTypeArguments[0];

                    // Десериализуем объект в тип payloadType
                    object? deserializedObject = JsonConvert.DeserializeObject(json, payloadType);

                    if (deserializedObject != null)
                    {
                        // Вызываем метод Handle с правильным типом
                        System.Reflection.MethodInfo? handleMethod = handler.GetType().GetMethod(nameof(IMessageHandler<object>.Handle));
                        if (handleMethod != null)
                        {
                            _logger.LogInformation($"Вызов обработчика {handleMethod.DeclaringType?.FullName}");
                            object? taskObject = handleMethod.Invoke(handler, new[] { deserializedObject });
                            if (taskObject != null && taskObject is Task task)
                            {
                                await task;
                            }
                            else
                            {
                                throw new HandlerFactoryException();
                            }
                        }
                    }
                }
            }
        }
    }

    public class HandlerFactoryException : Exception
    {

    }
}
