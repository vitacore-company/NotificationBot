using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotificationsBot.Handlers;
public interface ITelegramCommandHandler
{
    Task HandleOnUpdate(Message msg, UpdateType type);
}