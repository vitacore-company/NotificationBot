using NotificationsBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotificationsBot.Handlers;

/// <summary>
/// <inheritdoc cref="NotificationsBot.Handlers.ITelegramCommandHandler"/>
/// </summary>
/// <seealso cref="NotificationsBot.Handlers.ITelegramCommandHandler" />
/// <seealso cref="Telegram.Bot.Polling.IUpdateHandler" />
public class TelegramCommandHandler : ITelegramCommandHandler, IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUsersDataService _usersDataService;

    public TelegramCommandHandler(ITelegramBotClient botClient, IUsersDataService usersDataService)
    {
        _botClient = botClient;
        _usersDataService = usersDataService;
    }

    /// <summary>
    /// Обрабатывает <see cref="T:Telegram.Bot.Types.Update" />.
    /// </summary>
    /// <param name="botClient">Экземпляр <see cref="T:Telegram.Bot.ITelegramBotClient" /> бота, получающего <see cref="T:Telegram.Bot.Types.Update" /></param>
    /// <param name="update"><see cref="T:Telegram.Bot.Types.Update" /> для обработки</param>
    /// <param name="cancellationToken">Токен <see cref="T:System.Threading.CancellationToken" />, который будет уведомлять о том, что выполнение метода должно быть отменено</param>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await HandleOnUpdate(update.Message, update.Type);
    }

    /// <summary>
    /// Обработка обновлений в чате бота
    /// </summary>
    /// <param name="msg">Сообщение.</param>
    /// <param name="type">Тип обновления в чате.</param>
    public async Task HandleOnUpdate(Message msg, UpdateType type)
    {

        switch (type)
        {
            case UpdateType.Unknown:
                break;
            case UpdateType.Message:
                await HandleOnMessage(msg);
                break;
            case UpdateType.InlineQuery:
                break;
            case UpdateType.ChosenInlineResult:
                break;
            case UpdateType.CallbackQuery:
                break;
            case UpdateType.EditedMessage:
                break;
            case UpdateType.ChannelPost:
                break;
            case UpdateType.EditedChannelPost:
                break;
            case UpdateType.ShippingQuery:
                break;
            case UpdateType.PreCheckoutQuery:
                break;
            case UpdateType.Poll:
                break;
            case UpdateType.PollAnswer:
                break;
            case UpdateType.MyChatMember:
                break;
            case UpdateType.ChatMember:
                break;
            case UpdateType.ChatJoinRequest:
                break;
            case UpdateType.MessageReaction:
                break;
            case UpdateType.MessageReactionCount:
                break;
            case UpdateType.ChatBoost:
                break;
            case UpdateType.RemovedChatBoost:
                break;
            case UpdateType.BusinessConnection:
                break;
            case UpdateType.BusinessMessage:
                break;
            case UpdateType.EditedBusinessMessage:
                break;
            case UpdateType.DeletedBusinessMessages:
                break;
            case UpdateType.PurchasedPaidMedia:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Обрабатывает сообщение.
    /// </summary>
    /// <param name="msg">Cообщение</param>
    private async Task HandleOnMessage(Message msg)
    {
        switch (msg.Text)
        {
            case "/start":
                await _botClient.SendMessage(msg.Chat, "Hello, World!");
                if (! await _usersDataService.IsContainUser(msg.Chat.Id))
                    await _usersDataService.SaveNewUser(null, msg.Chat.Id);
                break;
            case "/register":
                await _botClient.SendMessage(msg.Chat, "Enter your login");
                await _usersDataService.ChangeStatus(msg.Chat.Id, "/register");
                break;
            default:
                await HandleOnMessageWithState(msg);
                break;
        }
        //return Task.CompletedTask;
    }

    /// <summary>
    /// Обрабатывает сообщение с проверкой состояния.
    /// </summary>
    /// <param name="msg">Сообщение</param>
    private async Task HandleOnMessageWithState(Message msg)
    {
        string status = await _usersDataService.GetStatus(msg.Chat.Id);
        switch (status)
        {
            case "/register":
                if (_usersDataService.IsContainUser(msg.Chat.Id).Result)
                {
                    await _usersDataService.UpdateUser(msg.Text, msg.Chat.Id);
                    await _botClient.SendMessage(msg.Chat, "SUper!!!");
                    await _usersDataService.CancelStatus(msg.Chat.Id);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Обрабатывает исключение <see cref="T:System.Exception" />
    /// </summary>
    /// <param name="botClient">Экземпляр <see cref="T:Telegram.Bot.ITelegramBotClient" /> бота, получившего <see cref="T:System.Exception" /></param>
    /// <param name="exception">Экземпляр <see cref="T:System.Exception" /> для обработки</param>
    /// <param name="source">Место возникновения ошибки</param>
    /// <param name="cancellationToken">Токен <see cref="T:System.Threading.CancellationToken" />, который будет уведомлять о том, что выполнение метода должно быть отменено</param>.
    /// <returns></returns>
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
