using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using TelegramBot.Enums;
using Telegram.Bot;
using TelegramBot.Repo;

namespace TelegramBot.MHandlers
{
    public class UHandlers
    {
        static string firstName;
        static string lastName;
        static string phoneNumber;
        static string city;
        static string street;
        static UserRoles userRole = UserRoles.Default;
        static string houseNumber;

        public static async void HandleCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var callbackData = callbackQuery.Data;

            // Обработка нажатия на кнопку
            switch (callbackData)
            {
                case "admin":
                    userRole = UserRoles.Admin;
                    break;
                case "executor":
                    userRole = UserRoles.Executor;
                    break;
                case "default":
                    userRole = UserRoles.Default;
                    break;
            }
            //await Handlers._botClient.SendTextMessageAsync(chatId, "Введите имя:");
            Handlers._botClient.OnCallbackQuery -= HandleCallbackQuery;
            Handlers._botClient.OnMessage += HandleFirstNameInput;
        }

        public static async void HandleManipulationUsersCallbackQuery(
            object sender,
            CallbackQueryEventArgs e
        )
        {
            var callbackQuery = e.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var callbackData = callbackQuery.Data;

            if (callbackData.StartsWith("delete"))
            {
                var userId = int.Parse(callbackData.Split(" ")[1]);
                ADODB.DeleteUser(userId);
                await Handlers._botClient.SendTextMessageAsync(
                    chatId,
                    $"Пользователь с ID {userId} был успешно удален ❌"
                );
            }
            Handlers._botClient.OnCallbackQuery -= HandleManipulationUsersCallbackQuery;
            Handlers._botClient.OnMessage += HandleFirstNameInput;
        }

        public static async void HandleFirstNameInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                firstName = e.Message.Text;
                if (string.IsNullOrEmpty(firstName))
                {
                    // Проверяем, что имя пользователя не пустое
                    await Handlers._botClient.SendTextMessageAsync(
                        chatId,
                        "Имя пользователя не может быть пустым. Пожалуйста, введите имя пользователя:"
                    );
                }
                else
                {
                    await Handlers._botClient.SendTextMessageAsync(chatId, "Введите фамилию:");
                    Handlers._botClient.OnMessage -= HandleFirstNameInput;
                    Handlers._botClient.OnMessage += HandleLastNameInput;
                }
            }
        }

        private static async void HandleLastNameInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                lastName = e.Message.Text;
                await Handlers._botClient.SendTextMessageAsync(
                    chatId,
                    "Введите телефона для связи"
                );
                Handlers._botClient.OnMessage -= HandleLastNameInput;
                Handlers._botClient.OnMessage += HandlePhoneNumberInput;
            }
        }

        private static async void HandlePhoneNumberInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                phoneNumber = e.Message.Text;
                await Handlers._botClient.SendTextMessageAsync(chatId, "Введите город:");
                Handlers._botClient.OnMessage -= HandlePhoneNumberInput;
                Handlers._botClient.OnMessage += HandleCityInput;
            }
        }

        private static async void HandleCityInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                city = e.Message.Text;
                await Handlers._botClient.SendTextMessageAsync(chatId, "Введите улицу:");
                Handlers._botClient.OnMessage -= HandleCityInput;
                Handlers._botClient.OnMessage += HandleStreetInput;
            }
        }

        private static async void HandleStreetInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                street = e.Message.Text;
                await Handlers._botClient.SendTextMessageAsync(chatId, "Введите номер дома:");
                Handlers._botClient.OnMessage -= HandleStreetInput;
                Handlers._botClient.OnMessage += HandleHouseNumberInput;
            }
        }

        private static async void HandleUserRoleInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                var userRoleText = e.Message.Text;
                UserRoles role;
                if (Enum.TryParse(userRoleText, out role))
                {
                    var keyboardRole = Handlers.GetButtonRoleUser();
                    await Handlers._botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите роль пользователя в системе",
                        replyMarkup: keyboardRole
                    );

                    Handlers._botClient.OnMessage -= HandleUserRoleInput;
                    Handlers._botClient.OnCallbackQuery += HandleCallbackQuery;
                }
                else
                {
                    await Handlers._botClient.SendTextMessageAsync(
                        chatId,
                        "Некорректное значение роли. Пожалуйста, выберите одну из предложенных ролей."
                    );
                }
            }
        }

        private static async void HandleHouseNumberInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                houseNumber = e.Message.Text;
                var newUser = new TelegramBot.Models.User
                {
                    UserRole = userRole,
                    ChatID = chatId,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    City = city,
                    Street = street,
                    HouseNumber = houseNumber
                };
                ADODB.AddUsers(newUser, chatId);
                var buttonsUser = Handlers.GetButtonDefaultUser();
                await Handlers._botClient.SendTextMessageAsync(
                    chatId,
                    "Ваша регистрация завершена ✅ Если Вы Исполнитель, дождитесь подтверждения от Администратора",
                    replyMarkup: buttonsUser
                );
                Handlers._botClient.OnMessage -= HandleHouseNumberInput;
                Handlers._botClient.OnMessage += Handlers.Default_OnMessageHandler;
            }
        }
    }
}
