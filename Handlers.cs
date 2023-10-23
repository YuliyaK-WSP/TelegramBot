using System;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Repo;
using TelegramBot.Models;
using TelegramBot.MHandlers;

namespace TelegramBot
{
    public static class Handlers
    {
        public static TelegramBotClient _botClient;

        public static async void Bot_OnMessageHandler(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                var message = e.Message.Text.ToLower();
                switch (message)
                {
                    case "/start":
                        var userRole = ADODB.GetUserRole(chatId);
                        Console.WriteLine("я работаю");
                        Console.WriteLine(userRole);
                        if (userRole == "admin")
                        {
                            var buttons = GetButtonMain();
                            await _botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:",
                                replyMarkup: buttons
                            );
                        }
                        if (userRole == "executor")
                        {
                            var buttonsExecutor = GetButtonDefaultExecutor();
                            await _botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:",
                                replyMarkup: buttonsExecutor
                            );
                        }
                        if (userRole == "default")
                        {
                            var buttonsUser = GetButtonDefaultUser();
                            await _botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:",
                                replyMarkup: buttonsUser
                            );
                        }
                        break;

                    case "добавить пользователя":

                        var buttonUserRole = GetButtonRoleUser();
                        await _botClient.SendTextMessageAsync(
                            chatId,
                            "Выберите роль пользователя в системе",
                            replyMarkup: buttonUserRole
                        );
                        _botClient.OnMessage -= Bot_OnMessageHandler;
                        _botClient.OnCallbackQuery += UHandlers.HandleCallbackQuery;
                        break;
                    case "пользователи":
                        ShowUsersCards(_botClient, chatId);
                        break;
                    case "оставить заявку":
                        var userSpecialistButton = GetButtonSpecialist();
                        await _botClient.SendTextMessageAsync(
                            chatId,
                            "Выберите специалиста",
                            replyMarkup: userSpecialistButton
                        );
                        _botClient.OnMessage -= Bot_OnMessageHandler;
                        _botClient.OnCallbackQuery += RHandlers.HandleCallbackQuery;
                        break;
                }
            }
        }

        

        public static async void ShowUsersCards(TelegramBotClient botClient, long chatId)
        {
            var users = ADODB.GetUsers();
            foreach (var user in users)
            {
                var userManipulationButton = GetButtonManipulationUser();
                var userInfo =
                    $"Имя:{user.FirstName}\nФамилия:{user.LastName}\nОтчество{user.PhoneNumber}";
                await _botClient.SendTextMessageAsync(
                    chatId,
                    userInfo,
                    replyMarkup: userManipulationButton
                );
            }
        }

        public static InlineKeyboardMarkup GetButtonRoleUser()
        {
            var keyboardRole = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton { Text = "Администратор", CallbackData = "admin" },
                        new InlineKeyboardButton { Text = "Исполнитель", CallbackData = "executor" },
                        new InlineKeyboardButton { Text = "Пользователь", CallbackData = "default" }
                    }
                }
            );
            return keyboardRole;
        }
		public static InlineKeyboardMarkup GetButtonSpecialist()
        {
            var keyboardSpecialist = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton { Text = "Сантехник", CallbackData = "plumber" },
                        new InlineKeyboardButton { Text = "Электрик", CallbackData = "electrician" }
                    }
                }
            );
            return keyboardSpecialist;
        }

        private static ReplyKeyboardMarkup GetButtonMain()
        {
            var replyMarkup = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("Добавить пользователя"),
                        new KeyboardButton("Пользователи")
                    },
                    new[] { new KeyboardButton("Заявки"), new KeyboardButton("Button 4") }
                }
            };
            return replyMarkup;
        }

        private static ReplyKeyboardMarkup GetButtonDefaultExecutor()
        {
            var replyMarkup = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("Новые заявки"),
                        new KeyboardButton("Заявки в работе")
                    }
                }
            };
            return replyMarkup;
        }

        private static ReplyKeyboardMarkup GetButtonDefaultUser()
        {
            var replyMarkup = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new[]
                    {
                        new KeyboardButton("Оставить заявку"),
                        new KeyboardButton("Мои заявки")
                    },
                    new[] { new KeyboardButton("Контакты ТСЖ") }
                }
            };
            return replyMarkup;
        }

        private static InlineKeyboardMarkup GetButtonRequests()
        {
            var replyMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("В работу"),
                        InlineKeyboardButton.WithCallbackData("Отменить")
                    }
                }
            );

            return replyMarkup;
        }

        private static InlineKeyboardMarkup GetButtonManipulationUser()
        {
            var replyMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Редактировать"),
                        InlineKeyboardButton.WithCallbackData("Удалить")
                    }
                }
            );

            return replyMarkup;
        }
    }
}
