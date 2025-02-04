using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;

namespace NotificationsBot.Interfaces;

public interface INotificationService
{
    public Task Notify(Event eventNotification);
}
