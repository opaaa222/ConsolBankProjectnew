using System.Collections.Generic;
using BankProject.Domain.Models;

namespace BankProject.Domain.Interfaces
{
    public interface ITransactionLogger
    {
        void LogTransaction(int clientId, string type, decimal amount, decimal balanceAfter);
        List<Transaction> GetByClientId(int clientId);
    }
}