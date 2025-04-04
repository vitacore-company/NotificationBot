using Telegram.Bot;
using Telegram.Bot.Polling;

namespace NotificationsBot.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <seealso cref="ReceiverServiceBase&lt;IUpdateHandler&gt;" />
public class ReceiverService(ITelegramBotClient botClient, IUpdateHandler updateHandler, ILogger<ReceiverServiceBase<IUpdateHandler>> logger)
    : ReceiverServiceBase<IUpdateHandler>(botClient, updateHandler, logger);