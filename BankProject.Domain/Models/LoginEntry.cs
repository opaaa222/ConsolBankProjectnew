using System;

namespace BankProject.Domain.Models
{
    public class LoginEntry
    {    
        public int Id { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public DateTime LoginTime { get; set; }
    }
}