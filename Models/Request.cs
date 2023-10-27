using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.Models;
using TelegramBot.Enums;

namespace TelegramBot.Models
{
    public class Request
    {
        public int Number { get; set; }
        public Specialist Specialist { get; set; }
		public string SpecialistName{get;set;}
        public string Description { get; set; }
        public string Urgency { get; set; }
        public RequestStatus Status { get; set; }
        public string Assignee { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public int WorkRating { get; set; }
    }
}
