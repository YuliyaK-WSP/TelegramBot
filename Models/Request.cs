using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class Request
    {
        public int Number { get; set; }
        public string Specialist { get; set; }
        public string Description { get; set; }
        public string Urgency { get; set; }
        public string Status { get; set; }
        public string Assignee { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public int WorkRating { get; set; }
    }
}
