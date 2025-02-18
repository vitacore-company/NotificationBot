using Telegram.Bot.Polling;
using Telegram.Bot;

namespace NotificationsBot.Interfaces.Impl;

/// <summary>
/// Сервис приема <seealso cref="Telegram.Bot.Types.Update">обновлений</seealso> обновлений из чата 
/// </summary>
/// <typeparam name="TUpdateHandler">The type of the update handler.</typeparam>
/// <seealso cref="NotificationsBot.Interfaces.IReceiverService" />
public abstract class ReceiverServiceBase<TUpdateHandler>(ITelegramBotClient botClient, TUpdateHandler updateHandler, ILogger<ReceiverServiceBase<TUpdateHandler>> logger)
    : IReceiverService where TUpdateHandler : IUpdateHandler
{
    /// <summary>
    /// Старт обслуживания <seealso cref="Telegram.Bot.Types.Update">обновлений</seealso> с помощью предоставленного <seealso cref="IUpdateHandler">класса</seealso> обработчика обновлений
    /// </summary>.
    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        // ToDo: we can inject ReceiverOptions through IOptions container
        var receiverOptions = new ReceiverOptions() { DropPendingUpdates = true, AllowedUpdates = [] };

        var me = await botClient.GetMe(stoppingToken);
        logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "My Awesome Bot");

        // Start receiving updates
        await botClient.ReceiveAsync(updateHandler, receiverOptions, stoppingToken);
    }
}
