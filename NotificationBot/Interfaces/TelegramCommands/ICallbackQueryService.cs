using Telegram.Bot.Types;

namespace NotificationsBot.Interfaces.TelegramCallback
{
    public interface ICallbackQueryService
    {
        Task HandleCallbackQuery(CallbackQuery callbackQuery, Message message);
    }
}
