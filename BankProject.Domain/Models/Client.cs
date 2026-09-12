using System;
using System.Collections.Generic;
using System.Text;

namespace BankProject.Domain.Models
{
    public class Client : User
    {
        public decimal Balance { get; set; }
        public string LoadStatus { get; set; }
        public decimal LoanAmount { get; set; }

        public Client(int id, string userName, string password, string email, string role, decimal balance, string loadStatus, decimal loanAmount)
            : base(id, userName, password, email, role)
        {
            Balance = balance;
            LoadStatus = loadStatus;
            LoanAmount = loanAmount;
        }

    }
}
