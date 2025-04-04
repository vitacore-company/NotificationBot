using NotificationsBot.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
    private readonly IExistUserChecker _userChecker;
    private readonly INotificationTypesService _notificationTypesService;
    private readonly ILogger<TelegramCommandHandler> _logger;

    public TelegramCommandHandler(ITelegramBotClient botClient,
        IUsersDataService usersDataService,
        IExistUserChecker userChecker,
        INotificationTypesService notificationTypesService,
        ILogger<TelegramCommandHandler> logger)
    {
        _botClient = botClient;
        _usersDataService = usersDataService;
        _userChecker = userChecker;
        _notificationTypesService = notificationTypesService;
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
        switch (type)
        {
            case UpdateType.Unknown:
                break;
            case UpdateType.Message:
                if (msg != null)
                {
                    await HandleOnMessage(msg);
                }
                break;
            case UpdateType.InlineQuery:
                break;
            case UpdateType.ChosenInlineResult:
                break;
            case UpdateType.CallbackQuery:
                {
                    if (update.CallbackQuery != null)
                    {
                        Message? message = update.CallbackQuery.Message;

                        if (message == null)
                            return;

                        switch (update.CallbackQuery.Data)
                        {
                            case "loginChangeButton":
                                {
                                    await _botClient.SendMessage(message.Chat, "Введите новый логин (пример DEV\\Name.Surname)");
                                    await _usersDataService.ChangeStatus(message.Chat.Id, "/changeLogin");
                                }
                                break;

                            case "registerButton":
                                {
                                    await _botClient.SendMessage(message.Chat, "Введите логин (пример DEV\\Name.Surname)");
                                }
                                break;

                            case "notificationSettings":
                                {
                                    List<string> projects = _notificationTypesService.GetProjects();

                                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
                                    foreach (string project in projects)
                                    {
                                        inlineKeyboard.AddButton(new InlineKeyboardButton(project, project));
                                    }

                                    await _botClient.EditMessageReplyMarkup(
                                        message.Chat.Id,
                                        message.Id,
                                        replyMarkup: inlineKeyboard);
                                }
                                break;
                        }

                        if (await _notificationTypesService.GetProjectByName(update.CallbackQuery.Data ?? ""))
                        {
                            List<string> types = await _notificationTypesService.GetNotifications(message.Chat.Id, update.CallbackQuery.Data ?? "");

                            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
                            foreach (string _type in types)
                            {
                                inlineKeyboard.AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(_type, $"{_type.Substring(1)}") });
                            }

                            await _botClient.SendMessage(
                                message.Chat.Id,
                                $"Настройка оповещений для проекта {Markdown.Escape(update.CallbackQuery.Data)}",
                                parseMode: ParseMode.MarkdownV2,
                                replyMarkup: inlineKeyboard);
                        }
                        else if (message.Text?.Contains("Настройка оповещений") ?? false)
                        {
                            Match projectMatch = Regex.Match(message.Text, @"([\w.-]+(\([\w.-]+\))?)$");

                            if (projectMatch.Success)
                            {
                                string project = projectMatch.Groups[1].Value;
                                long chatId = message.Chat.Id;
                                string command = update.CallbackQuery.Data ?? "";

                                List<string> newNotifys = await _notificationTypesService.SetOrDeleteChatProjectNotification(project, chatId, command);

                                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
                                foreach (string _type in newNotifys)
                                {
                                    inlineKeyboard.AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(_type, $"{_type.Substring(1)}") });
                                }

                                await _botClient.EditMessageReplyMarkup(
                                    message.Chat.Id,
                                    message.Id,
                                    inlineKeyboard);
                            }
                        }

                        await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id);
                    }
                }
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
        try
        {
            _logger.LogInformation($"Вызвана команда {msg.Text} пользователем {msg.Chat.FirstName}{msg.Chat.LastName}, имя пользователя {msg.Chat.Username}");

            switch (msg.Text)
            {
                case "/start":
                    {
                        if (await _userChecker.CheckExistUser(msg.From?.Id ?? -1))
                        {
                            if (!await _usersDataService.IsContainUser(msg.Chat.Id))
                            {
                                await _usersDataService.SaveNewUser(null, msg.Chat.Id, msg.From?.Id ?? -1);
                                await _usersDataService.ChangeStatus(msg.Chat.Id, "/register");

                                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                                new List<InlineKeyboardButton[]>()
                                {
                                new InlineKeyboardButton[] // тут создаем массив кнопок
                                {
                                     InlineKeyboardButton.WithCallbackData("Регистрация", "registerButton"),
                                }
                                });

                                await _botClient.SendMessage(
                                    msg.Chat.Id,
                                    "Выберите действие",
                                    parseMode: ParseMode.MarkdownV2,
                                    replyMarkup: inlineKeyboard);
                            }
                            else
                            {
                                await _botClient.SendMessage(msg.Chat, "Вы уже авторизированы");

                                await sendBaseInformation(msg);
                            }
                        }
                    }
                    break;

                case "/register":
                    await _botClient.SendMessage(msg.Chat, "Enter your login");
                    await _usersDataService.ChangeStatus(msg.Chat.Id, "/register");
                    break;

                case "/help":
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine("*Обновление пуллреквеста*");
                        sb.AppendLine(Markdown.Escape("Тип оповещения, который уведомляет всех ревьюеров, которые находятся в пуллреквесте, о том, что произошли изменения (запушили код, аппрувнули и т.п.)"));
                        sb.AppendLine();
                        sb.AppendLine("*Комментирование пуллреквеста*");
                        sb.AppendLine(Markdown.Escape("Тип оповещения, который уведомляет всех ревьюеров, которые находятся в пуллреквесте, о том, что в пуллреквесте появились комментарии"));
                        sb.AppendLine();
                        sb.AppendLine("*Создание пуллреквеста*");
                        sb.AppendLine(Markdown.Escape("Тип оповещения, который уведомляет всех ревьюеров, которые находятся в пуллреквесте, о том, что был открыт пуллреквест"));
                        sb.AppendLine();
                        sb.AppendLine("*Создание рабочего элемента*");
                        sb.AppendLine(Markdown.Escape("Тип оповещения, который уведомляет о создании рабочего элемента человека, на которого он сразу был назначен"));
                        sb.AppendLine();
                        sb.AppendLine("*Обновление рабочего элемента*");
                        sb.AppendLine(Markdown.Escape("Тип оповещения, который уведомляет о изменении назначения или приоритета человека, на которого назначен рабочий элемент"));
                        sb.AppendLine();
                        sb.AppendLine("*Смена состояния сборки*");
                        sb.AppendLine(Markdown.Escape("Тип оповещения, который уведомляет о изменении состояния сборки человека, который открыл пуллреквест"));
                        sb.AppendLine();
                        sb.AppendLine(char.ConvertFromUtf32(0x2757) + Markdown.Escape("Оповещения не приходят тому, кто триггернул") + char.ConvertFromUtf32(0x2757));

                        await _botClient.SendMessage(msg.Chat, sb.ToString(), ParseMode.MarkdownV2);
                    }
                    break;

                default:
                    await HandleOnMessageWithState(msg);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
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
                if (_usersDataService.IsContainUser(msg.Chat.Id).Result && msg.Text != null)
                {
                    await _usersDataService.UpdateUser(msg.Text, msg.Chat.Id);
                    await _botClient.SendMessage(msg.Chat, "Вы успешно авторизировались");
                    await _usersDataService.CancelStatus(msg.Chat.Id);

                    _logger.LogInformation($"Пользователь {msg.Chat.FirstName}{msg.Chat.LastName}, имя пользователя {msg.Chat.Username} зарегестрирован");

                    await sendBaseInformation(msg);
                }
                break;

            case "/changeLogin":
                {
                    _logger.LogInformation($"Пользователь {msg.Chat.FirstName}{msg.Chat.LastName}, имя пользователя {msg.Chat.Username} изменил логин");
                    await _usersDataService.UpdateUser(msg.Text, msg.Chat.Id, msg.From?.Id ?? -1);
                    await _botClient.SendMessage(msg.Chat, "Логин изменен");
                    await _usersDataService.CancelStatus(msg.Chat.Id);
                }
                break;
            default:
                break;
        }
    }

    private async Task sendBaseInformation(Message msg)
    {
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
            new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[] // тут создаем массив кнопок
                {
                     InlineKeyboardButton.WithCallbackData("Изменить логин", "loginChangeButton"),
                     InlineKeyboardButton.WithCallbackData("Настройка оповещений", "notificationSettings"),
                }
            });

        await _botClient.SendMessage(
            msg.Chat.Id,
            "Выберите действие",
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: inlineKeyboard);
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
