using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBot;
using TelegramBot.Models;
using TelegramBot.Enums;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class User
    {
     	public int Id { get; set; }
		public bool ActiveRegist{get;set;}
		public long ChatID { get; set; }
		public UserRoles UserRole {get;set;}
        public string FirstName { get; set; }
		public string LastName{get;set;}
        public string Patronymic { get; set; }
		public string PhoneNumber {get;set;}
        public string City  { get; set; }
		public string Street { get; set; }
		public string HouseNumber { get; set; }
		public Specialist Specialist { get; set; }
    }
}