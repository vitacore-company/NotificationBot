using Telegram.Bot.Types;

namespace NotificationsBot.Interfaces.TelegramCallback
{
    public interface ICommandService
    {
        Task HandleStartCommand(Message msg);
        Task HandleSetCommand(Message msg);
        Task HandleHelpCommand(Message msg);
        Task HandleDefaultCommand(Message msg);
        Task HandleMessageWithState(Message msg);
    }
}
