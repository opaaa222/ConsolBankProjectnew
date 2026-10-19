using System;

namespace BankProject.Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Type { get; set; }        
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime Date { get; set; }
    }
}