using Microsoft.AspNet.WebHooks.Payloads;
using Microsoft.AspNetCore.Mvc;
using NotificationsBot.Interfaces;
using System.Text.Json;

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
    public async Task<ActionResult> Notification([FromBody] JsonElement value)
    {
        string eventType = value.GetProperty("eventType").ToString();

        await _notificationService.Notify(value, eventType);
        return Accepted();
    }
}