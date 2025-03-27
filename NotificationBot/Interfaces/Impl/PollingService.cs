namespace NotificationsBot.Interfaces.Impl;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <seealso cref="NotificationsBot.Interfaces.Impl.PollingServiceBase&lt;NotificationsBot.Interfaces.Impl.ReceiverService&gt;" />
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
