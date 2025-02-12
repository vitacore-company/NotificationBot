using Telegram.Bot;
using Telegram.Bot.Polling;

namespace NotificationsBot.Interfaces.Impl;

public class ReceiverService(ITelegramBotClient botClient, IUpdateHandler updateHandler, ILogger<ReceiverServiceBase<IUpdateHandler>> logger)
    : ReceiverServiceBase<IUpdateHandler>(botClient, updateHandler, logger);