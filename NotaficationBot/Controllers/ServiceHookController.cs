using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using NotificationsBot.Interfaces;

namespace NotificationsBot.Controllers;

[Route("api/service-hook/[controller]")]
[ApiController]
public class ServiceHookController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public ServiceHookController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost]
    public void Notification([FromBody] Event value)
    {
        _notificationService.Notify(value);
    }
}
