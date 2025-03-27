using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotificationsBot.Handlers;
/// <summary>
/// Обработчик команд отправленных боту
/// </summary>
public interface ITelegramCommandHandler
{
    /// <summary>
    /// Обработка обновлений в чате бота
    /// </summary>
    /// <param name="msg">Сообщение.</param>
    /// <param name="type">Тип обновления в чате.</param>
    /// <returns></returns>
    Task HandleOnUpdate(Message msg, UpdateType type, Update update);
}