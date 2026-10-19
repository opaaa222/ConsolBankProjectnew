using System.Collections.Generic;
using BankProject.Domain.Models;

namespace BankProject.Service.Interfaces
{
    public interface IAtmService
    {
        Client GetClientById(int clientId);
        List<Transaction> GetTransactionHistory(int clientId);
        bool Deposit(int clientId, decimal amount);
        bool Withdraw(int clientId, decimal amount);
        bool RequestLoan(int clientId, decimal amount, decimal salary);
        List<Client> GetPendingLoans();
        bool ProcessLoan(int clientId, bool isApproved);
        bool TransferMoney(int senderId, string recipientIdentifier, decimal amount);
        List<Client> GetAllClients();
        bool SetClientBlockedStatus(int clientId, bool isBlocked);
    }
}