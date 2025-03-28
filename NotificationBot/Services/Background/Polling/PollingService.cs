namespace NotificationsBot.Services.Background.Polling;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <seealso cref="PollingServiceBase&lt;ReceiverService&gt;" />
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
