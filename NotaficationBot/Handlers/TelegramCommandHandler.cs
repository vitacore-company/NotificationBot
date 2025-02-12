using NotificationsBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotificationsBot.Handlers;

public class TelegramCommandHandler : ITelegramCommandHandler, IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUsersDataService _usersDataService;

    public TelegramCommandHandler(ITelegramBotClient botClient, IUsersDataService usersDataService)
    {
        _botClient = botClient;
        _usersDataService = usersDataService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await HandleOnUpdate(update.Message, update.Type);
    }

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

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
