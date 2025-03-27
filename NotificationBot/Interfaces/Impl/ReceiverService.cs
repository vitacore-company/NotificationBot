using Telegram.Bot;
using Telegram.Bot.Polling;

namespace NotificationsBot.Interfaces.Impl;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <seealso cref="NotificationsBot.Interfaces.Impl.ReceiverServiceBase&lt;Telegram.Bot.Polling.IUpdateHandler&gt;" />
public class ReceiverService(ITelegramBotClient botClient, IUpdateHandler updateHandler, ILogger<ReceiverServiceBase<IUpdateHandler>> logger)
    : ReceiverServiceBase<IUpdateHandler>(botClient, updateHandler, logger);