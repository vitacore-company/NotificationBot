using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Text.Json;

namespace NotificationsBot.Interfaces;

/// <summary>
/// Сервис уведомлений 
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Уведомляет об указанном событии.
    /// </summary>
    /// <param name="eventNotification">Уведомление о событии.</param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/azure/devops/service-hooks/events?view=azure-devops">Типы событий</see>
    /// </remarks>
    public Task Notify(JsonElement element, string eventType);
}
