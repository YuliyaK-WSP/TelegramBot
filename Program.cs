using System;
using System.Configuration;
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
    class Program
    {
        
        //static TelegramBotClient _botClient;

        static void Main(string[] args)
        {
            Handlers._botClient = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramToken"]);
            Handlers._botClient.OnMessage += Handlers.Bot_OnMessageHandler;
            Handlers._botClient.StartReceiving();
            Console.WriteLine("Бот работает. Нажмите любую кнопку для выхода.");
            Console.ReadKey();
        }

        

        
    }
}
