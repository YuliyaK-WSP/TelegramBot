using System.Linq.Expressions;
namespace TelegramBot.Enums
{
    public enum RequestStatus
    {
        New,
		InWork,
		Cancelled,
		Completed,
		Archive
    }
}