using NotificationsBot.Interfaces;
using NotificationsBot.Models.Locals;
using System.Text.Json;

namespace NotificationsBot.Services;

/// <summary>
/// Сервис уведомления для телеграмм бота
/// </summary>
/// <seealso cref="INotificationService" />
public class TelegramNotificationService : INotificationService
{
    private readonly IHandlerFactory _handlerFactory;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(IHandlerFactory handlerFactory, ILogger<TelegramNotificationService> logger)
    {
        _handlerFactory = handlerFactory;
        _logger = logger;
    }

    public async Task Notify(JsonElement element, string eventType)
    {
        if (LocalMessageHandlers.Handlers.TryGetValue(eventType, out Type? handlerType))
        {
            await _handlerFactory.ProcessHandler(handlerType, element.ToString());
        }
    }
}


