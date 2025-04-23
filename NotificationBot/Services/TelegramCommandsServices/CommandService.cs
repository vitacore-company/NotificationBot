using NotificationsBot.Interfaces.TelegramCallback;
using NotificationsBot.Interfaces;
using System.Text;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotificationsBot.Services
{
    public class CommandService : ICommandService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUsersDataService _usersDataService;
        private readonly IExistUserChecker _userChecker;
        private readonly ILogger<CommandService> _logger;

        public CommandService(
            ITelegramBotClient botClient,
            IUsersDataService usersDataService,
            IExistUserChecker userChecker,
            ILogger<CommandService> logger)
        {
            _botClient = botClient;
            _usersDataService = usersDataService;
            _userChecker = userChecker;
            _logger = logger;
        }

        /// <summary>
        /// Метод, ответственный за работу с командой /start
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task HandleStartCommand(Message msg)
        {
            _logger.LogInformation($"Вызвана команда {msg.Text} пользователем {msg.Chat.FirstName}{msg.Chat.LastName}, имя пользователя {msg.Chat.Username}");

            switch (msg.Chat.Type)
            {
                case ChatType.Group:
                case ChatType.Supergroup:
                    {
                        await handleGroupStart(msg);
                    }
                    break;

                case ChatType.Private:
                    {
                        await handlePrivateStart(msg);
                    }
                    break;
            }
        }

        /// <summary>
        /// Метод, ответственный за работу с групповыми чатами
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task handleGroupStart(Message msg)
        {
            if (!await _usersDataService.IsContainUser(msg.Chat.Id))
            {
                await _usersDataService.SaveNewUser(msg.Chat.Title, msg.Chat.Id, msg.Chat.Id);
            }

            if (!await _usersDataService.IsContaionTopic(msg.MessageThreadId ?? -1, msg.Chat.Id))
            {
                await _usersDataService.SaveTopicToChatId(msg.MessageThreadId ?? -1, msg.Chat.Id);
            }

            await sendBaseInformation(msg, msg.MessageThreadId ?? -1);
        }

        /// <summary>
        /// Метод, ответственный за работу с приватными чатами
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task handlePrivateStart(Message msg)
        {
            if (await _userChecker.CheckExistUser(msg.From?.Id ?? -1))
            {
                if (!await _usersDataService.IsContainUser(msg.Chat.Id))
                {
                    await _usersDataService.SaveNewUser(null, msg.Chat.Id, msg.From?.Id ?? -1);
                    await _usersDataService.ChangeStatus(msg.Chat.Id, "/register");

                    var inlineKeyboard = new InlineKeyboardMarkup(
                        new List<InlineKeyboardButton[]>()
                        {
                        new InlineKeyboardButton[]
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
                    await sendBaseInformation(msg, null);
                }
            }
        }

        /// <summary>
        /// Метод обработки команды /set
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task HandleSetCommand(Message msg)
        {
            if (msg.Chat.Type is ChatType.Group or ChatType.Supergroup)
            {
                if (!string.IsNullOrEmpty(msg.Text))
                {
                    string project = msg.Text.Replace("/set ", "");
                    await _usersDataService.UpdateTopic(msg.MessageThreadId ?? -1, msg.Chat.Id, project);
                }
            }
        }

        /// <summary>
        /// Метод обработки вызова команды /help
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task HandleHelpCommand(Message msg)
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
            sb.AppendLine("*Оповещения АКУЗа*");
            sb.AppendLine(Markdown.Escape("Допустим, если включить уведомления NAO, будут приходить все уведомления со сборок в папке \\\\Regions\\\\NAO, так же со всеми остальными регионами"));
            sb.AppendLine(Markdown.Escape("Если включить Others, будут приходить оповещения о всех сборках, кроме регионов (больше нужно для тех, кто собирает версии)"));
            sb.AppendLine();
            sb.AppendLine(char.ConvertFromUtf32(0x2757) + Markdown.Escape("Оповещения не приходят тому, кто триггернул") + char.ConvertFromUtf32(0x2757));

            await _botClient.SendMessage(msg.Chat, sb.ToString(), ParseMode.MarkdownV2, messageThreadId: msg.MessageThreadId);
        }

        /// <summary>
        /// Метод обработки сообщения без команды
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task HandleDefaultCommand(Message msg)
        {
            await HandleMessageWithState(msg);
        }

        /// <summary>
        /// Метод обработки сообщения пользователя со статусом (не работает с группами)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task HandleMessageWithState(Message msg)
        {
            if (msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup)
            {
                return;
            }

            string status = await _usersDataService.GetStatus(msg.Chat.Id);
            switch (status)
            {
                case "/register":
                    {
                        if (_usersDataService.IsContainUser(msg.Chat.Id).Result && msg.Text != null)
                        {
                            await _usersDataService.UpdateUser(msg.Text, msg.Chat.Id);
                            await _botClient.SendMessage(msg.Chat, "Вы успешно авторизировались");
                            await _usersDataService.CancelStatus(msg.Chat.Id);

                            _logger.LogInformation($"Пользователь {msg.Chat.FirstName}{msg.Chat.LastName}, имя пользователя {msg.Chat.Username} зарегестрирован");

                            await sendBaseInformation(msg, null);
                        }
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
            }
        }

        /// <summary>
        /// Метод, который отсылает пользователю доступные ему команды
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        private async Task sendBaseInformation(Message msg, int? threadId)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();

            inlineKeyboard.AddButton(InlineKeyboardButton.WithCallbackData("Настройка оповещений", "notificationSettings"));

            if (msg.Chat.Type != ChatType.Group && msg.Chat.Type != ChatType.Supergroup)
            {
                inlineKeyboard.AddButton(InlineKeyboardButton.WithCallbackData("Изменить логин", "loginChangeButton"));
            }

            await _botClient.SendMessage(
                msg.Chat.Id,
                "Выберите действие",
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: inlineKeyboard,
                messageThreadId: threadId);
        }
    }
}
