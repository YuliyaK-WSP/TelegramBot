using System.Reflection.Metadata;
using System;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Repo;
using TelegramBot.Models;
using TelegramBot.Enums;
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
							Handlers._botClient.OnMessage -= Bot_OnMessageHandler;
                    		Handlers._botClient.OnMessage += Admin_OnMessageHandler;
                        }
                        else if (userRole == "executor")
                        {
                            var buttonsExecutor = GetButtonDefaultExecutor();
                            await _botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:",
                                replyMarkup: buttonsExecutor
                            );
                        }
                        else if (userRole == "default")
                        {
                            var buttonsUser = GetButtonDefaultUser();
                            await _botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:",
                                replyMarkup: buttonsUser
                            );
							Handlers._botClient.OnMessage -= Bot_OnMessageHandler;
                    		Handlers._botClient.OnMessage += Default_OnMessageHandler;
                        }
                        else
                        {
                            var buttonsUser = GetFirstButtonUser();
                            await _botClient.SendTextMessageAsync(
                                chatId,
                                "Выберите действие:",
                                replyMarkup: buttonsUser
                            );
							Handlers._botClient.OnMessage -= Bot_OnMessageHandler;
                    		Handlers._botClient.OnMessage += Register_OnMessageHandler;
                        }
                        break;
                    /*
                    
                    
                    
                    */
                }
            }
        }

		public static async void Admin_OnMessageHandler(object sender, MessageEventArgs e)
        {
			var chatId = e.Message.Chat.Id;
                var message = e.Message.Text.ToLower();
                switch (message)
                {
					case "добавить пользователя":
                        var buttonUserRole = GetButtonRoleUser();
                        await _botClient.SendTextMessageAsync(
                            chatId,
                            "Выберите роль пользователя в системе",
                            replyMarkup: buttonUserRole
                        );
                        _botClient.OnMessage -= Admin_OnMessageHandler;
                        _botClient.OnCallbackQuery += UHandlers.HandleCallbackQuery;
                        break;
					case "пользователи":
                        ShowUsersCards(_botClient, chatId);
                        break;
				}
		}

		public static async void Default_OnMessageHandler(object sender, MessageEventArgs e)
        {
			var chatId = e.Message.Chat.Id;
			var message = e.Message.Text.ToLower();
			switch (message)
			{
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
		
		public static async void Register_OnMessageHandler(object sender, MessageEventArgs e)
        {
			var chatId = e.Message.Chat.Id;
			var message = e.Message.Text.ToLower();
			switch (message)
			{
				case "регистрация":
					await _botClient.SendTextMessageAsync(chatId, "Введите имя:");
					_botClient.OnMessage -= Register_OnMessageHandler;
					_botClient.OnMessage += UHandlers.HandleFirstNameInput;
					break;
			}
		}

        public static async void ShowUsersCards(TelegramBotClient botClient, long chatId)
        {
            var users = ADODB.GetUsers();
            foreach (var user in users)
            {
                var userManipulationButton = GetButtonManipulationUser(user.Id);
                var userInfo =
                    $"Имя:{user.FirstName}\nФамилия:{user.LastName}\nОтчество{user.PhoneNumber}";
                await _botClient.SendTextMessageAsync(
                    chatId,
                    userInfo,
                    replyMarkup: userManipulationButton
                );
            }
			_botClient.OnCallbackQuery += UHandlers.HandleManipulationUsersCallbackQuery;
        }

        public static InlineKeyboardMarkup GetButtonRoleUser()
        {
            var keyboardRole = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Администратор", "admin"),
                        InlineKeyboardButton.WithCallbackData("Исполнитель", "executor"),
                        InlineKeyboardButton.WithCallbackData("Пользователь", "default")
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
                        InlineKeyboardButton.WithCallbackData("Сантехник", "plumber"),
                        InlineKeyboardButton.WithCallbackData("Электрик", "electrician")
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
                    new[] 
					{ 
						new KeyboardButton("Заявки"),
						new KeyboardButton("Button 4")
					}
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

        private static ReplyKeyboardMarkup GetFirstButtonUser()
        {
            var replyMarkup = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new[] { new KeyboardButton("Регистрация"), new KeyboardButton("Контакты ТСЖ") }
                }
            };
            return replyMarkup;
        }

        public static ReplyKeyboardMarkup GetButtonDefaultUser()
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

        private static InlineKeyboardMarkup GetButtonManipulationUser(int userId)
        {
            var replyMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Редактировать"),
                        InlineKeyboardButton.WithCallbackData("Удалить", $"delete {userId}")
                    }
                }
            );
            return replyMarkup;
        }
    }
}
