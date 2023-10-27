using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using TelegramBot.Enums;
using Telegram.Bot;
using Telegram.Bot.Args;
using TelegramBot.Repo;

namespace TelegramBot.MHandlers
{
    public class RHandlers
    {
        static Specialist specialist;
        static string description;

        public static async void HandleCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var callbackData = callbackQuery.Data;
            // Обработка нажатия на кнопку
            switch (callbackData)
            {
                case "plumber":
                    specialist = Specialist.Plumber;
                    break;
                case "electrician":
                    specialist = Specialist.Electrician;
                    break;
            }
            await Handlers._botClient.SendTextMessageAsync(chatId, "Коротко опишите проблему:");
            Handlers._botClient.OnCallbackQuery -= HandleCallbackQuery;
            Handlers._botClient.OnMessage += HandleRequestDescription;
        }

        public static async void HandleSpecialistInput(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                var specialistText = e.Message.Text;

                Specialist specialist;

                if (Enum.TryParse(specialistText, out specialist))
                {
                    var keyboardSpecialist = Handlers.GetButtonSpecialist();

                    await Handlers._botClient.SendTextMessageAsync(
                        chatId,
                        "Выберите специалиста",
                        replyMarkup: keyboardSpecialist
                    );

                    Handlers._botClient.OnMessage -= HandleSpecialistInput;
                    Handlers._botClient.OnCallbackQuery += HandleCallbackQuery;
                }
                else
                {
                    await Handlers._botClient.SendTextMessageAsync(
                        chatId,
                        "Некорректное значение специалиста. Пожалуйста, выберите одну из предложенных специализаций."
                    );
                }
            }
        }

        public static async void HandleManipulationRequestCallbackQuery(
            object sender,
            CallbackQueryEventArgs e
        )
        {
            var callbackQuery = e.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var callbackData = callbackQuery.Data;
            // Обработка нажатия на кнопку
            if (callbackData.StartsWith("inWork"))
            {
                var requestId = int.Parse(callbackData.Split(" ")[1]);
                bool inWork = true; // Инициализация переменной
                var buttonText = inWork ? "В работе" : "В работу";
                var buttonMarkup = Handlers.GetButtonManipulationRequest(requestId, inWork);
                await Handlers._botClient.EditMessageReplyMarkupAsync(
                    chatId,
                    callbackQuery.Message.MessageId,
                    replyMarkup: buttonMarkup
                );
                await Handlers._botClient.SendTextMessageAsync(
                    chatId,
                    $"Нажата кнопка \"{buttonText}\" для заявки с ID {requestId}"
                );
            }

            Handlers._botClient.OnCallbackQuery -= HandleManipulationRequestCallbackQuery;
            Handlers._botClient.OnMessage += UHandlers.HandleFirstNameInput;
        }

        public static async void HandleRequestDescription(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                description = e.Message.Text;

                var request = new TelegramBot.Models.Request
                {
                    Specialist = specialist,
                    Description = description
                };

                // Добавляем новую заявку
                ADODB.AddRequestUser(request);

                await Handlers._botClient.SendTextMessageAsync(
                    chatId,
                    "Ваша заявка успешно добавлена ✅"
                );

                Handlers._botClient.OnMessage -= HandleRequestDescription;
                Handlers._botClient.OnMessage += Handlers.Bot_OnMessageHandler;
            }
        }
    }
}
