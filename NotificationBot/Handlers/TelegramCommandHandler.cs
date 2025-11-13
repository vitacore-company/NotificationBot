using NotificationsBot.Interfaces.TelegramCallback;
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
    private readonly ICommandService _commandService;
    private readonly ICallbackQueryService _callbackQueryService;
    private readonly ILogger<TelegramCommandHandler> _logger;

    public TelegramCommandHandler(
        ITelegramBotClient botClient,
        ICommandService commandService,
        ICallbackQueryService callbackQueryService,
        ILogger<TelegramCommandHandler> logger)
    {
        _botClient = botClient;
        _commandService = commandService;
        _callbackQueryService = callbackQueryService;
        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает <see cref="T:Telegram.Bot.Types.Update" />.
    /// </summary>
    /// <param name="botClient">Экземпляр <see cref="T:Telegram.Bot.ITelegramBotClient" /> бота, получающего <see cref="T:Telegram.Bot.Types.Update" /></param>
    /// <param name="update"><see cref="T:Telegram.Bot.Types.Update" /> для обработки</param>
    /// <param name="cancellationToken">Токен <see cref="T:System.Threading.CancellationToken" />, который будет уведомлять о том, что выполнение метода должно быть отменено</param>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await HandleOnUpdate(update.Message, update.Type, update);
    }

    /// <summary>
    /// Обработка обновлений в чате бота
    /// </summary>
    /// <param name="msg">Сообщение.</param>
    /// <param name="type">Тип обновления в чате.</param>
    public async Task HandleOnUpdate(Message? msg, UpdateType type, Update update)
    {
        try
        {
            switch (type)
            {
                case UpdateType.Message:
                    if (msg != null)
                    {
                        await HandleMessage(msg);
                    }
                    break;

                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery != null && update.CallbackQuery.Message != null)
                    {
                        await _callbackQueryService.HandleCallbackQuery(update.CallbackQuery, update.CallbackQuery.Message);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    /// <summary>
    /// Обработка сообщений пользователей
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private async Task HandleMessage(Message msg)
    {
        string? botName = _botClient.GetMe().Result.Username;

        switch (msg.Text)
        {
            case string when msg.Text == $"/start@{botName}":
            case "/start":
                {
                    await _commandService.HandleStartCommand(msg);
                }
                break;

            case string when msg.Text.Contains("/set"):
                {
                    await _commandService.HandleSetCommand(msg);
                }
                break;

            case "/help":
            case string when msg.Text == $"/help@{botName}":
                {
                    await _commandService.HandleHelpCommand(msg);
                }
                break;

            default:
                {
                    await _commandService.HandleDefaultCommand(msg);
                }
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
