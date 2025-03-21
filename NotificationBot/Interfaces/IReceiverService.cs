namespace NotificationsBot.Interfaces;

/// <summary>
/// Сервис приема <seealso cref="Telegram.Bot.Types.Update">обновлений</seealso> обновлений из чата 
/// </summary>
/// <typeparam name="TUpdateHandler">The type of the update handler.</typeparam>
/// <seealso cref="NotificationsBot.Interfaces.IReceiverService" />
public interface IReceiverService
{
    /// <summary>
    /// Старт обслуживания <seealso cref="Telegram.Bot.Types.Update">обновлений</seealso> с помощью предоставленного <seealso cref="IUpdateHandler">класса</seealso> обработчика обновлений
    /// </summary>.
    Task ReceiveAsync(CancellationToken stoppingToken);
}
