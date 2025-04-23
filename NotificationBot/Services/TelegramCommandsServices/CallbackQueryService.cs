using NotificationsBot.Interfaces.TelegramCallback;
using NotificationsBot.Interfaces;
using System.Text.RegularExpressions;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace NotificationsBot.Services
{
    public class CallbackQueryService : ICallbackQueryService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUsersDataService _usersDataService;
        private readonly INotificationTypesService _notificationTypesService;

        public CallbackQueryService(
            ITelegramBotClient botClient,
            IUsersDataService usersDataService,
            INotificationTypesService notificationTypesService)
        {
            _botClient = botClient;
            _usersDataService = usersDataService;
            _notificationTypesService = notificationTypesService;
        }

        /// <summary>
        /// Общий обработчик всех типов нажатий кнопок пользователем
        /// </summary>
        /// <param name="callbackQuery"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task HandleCallbackQuery(CallbackQuery callbackQuery, Message message)
        {
            int? topic = message.MessageThreadId;

            if (message == null)
            {
                return;
            }

            switch (callbackQuery.Data)
            {
                case "loginChangeButton":
                    {
                        await HandleLoginChangeButton(message);
                    }
                    break;

                case "registerButton":
                    {
                        await _botClient.SendMessage(message.Chat, "Введите логин (пример DEV\\Name.Surname)");
                    }
                    break;

                case "notificationSettings":
                    {
                        await HandleNotificationSettings(message);
                    }
                    break;

                default:
                    {
                        await HandleProjectOrNotificationSelection(callbackQuery, message, topic);
                    }
                    break;
            }

            await _botClient.AnswerCallbackQuery(callbackQuery.Id);
        }

        /// <summary>
        /// Метод, отвечающий за смену логина
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task HandleLoginChangeButton(Message message)
        {
            await _botClient.SendMessage(message.Chat, "Введите новый логин (пример DEV\\Name.Surname)");
            await _usersDataService.ChangeStatus(message.Chat.Id, "/changeLogin");
        }

        /// <summary>
        /// Метод, который отдает проекты для настройки оповещений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task HandleNotificationSettings(Message message)
        {
            List<string> projects = _notificationTypesService.GetProjects();

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
            for (int i = 0; i < projects.Count; i++)
            {
                if (i % 3 == 0)
                {
                    inlineKeyboard.AddNewRow();
                }

                inlineKeyboard.AddButton(new InlineKeyboardButton(projects[i], projects[i]));
            }

            await _botClient.EditMessageReplyMarkup(
                message.Chat.Id,
                message.Id,
                replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// Метод, который отвечает за ветвление между настройкой оповещений или выбором проекта
        /// </summary>
        /// <param name="callbackQuery"></param>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        private async Task HandleProjectOrNotificationSelection(CallbackQuery callbackQuery, Message message, int? topic)
        {
            if (await _notificationTypesService.GetProjectByName(callbackQuery.Data ?? ""))
            {
                await HandleProjectSelection(callbackQuery, message, topic);
            }
            else if (message.Text?.Contains("Настройка оповещений") ?? false)
            {
                await HandleNotificationSelection(callbackQuery, message);
            }
        }

        /// <summary>
        /// Метод, который возвращает проекты для настройки оповещений
        /// </summary>
        /// <param name="callbackQuery"></param>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        private async Task HandleProjectSelection(CallbackQuery callbackQuery, Message message, int? topic)
        {
            List<string> types = await _notificationTypesService.GetNotifications(message.Chat.Id, callbackQuery.Data ?? "");

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
            foreach (string _type in types)
            {
                inlineKeyboard.AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(_type, $"{_type.Substring(1)}") });
            }

            await _botClient.SendMessage(
                message.Chat.Id,
                $"Настройка оповещений для проекта {Markdown.Escape(callbackQuery.Data)}",
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: inlineKeyboard,
                messageThreadId: topic);
        }

        /// <summary>
        /// Метод с настройкой типов оповещений
        /// </summary>
        /// <param name="callbackQuery"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task HandleNotificationSelection(CallbackQuery callbackQuery, Message message)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                Match projectMatch = Regex.Match(message.Text, @"([\w.-]+(\([\w.-]+\))?)$");

                if (projectMatch.Success)
                {
                    string project = projectMatch.Groups[1].Value;
                    long chatId = message.Chat.Id;
                    string command = callbackQuery.Data ?? "";

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
        }
    }
}
