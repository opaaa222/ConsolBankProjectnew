using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Service.Interfaces;
using BankProjects.Service.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankProjects.Service.implementation
{
    public class AtmService : IAtmService
    {
        private readonly IUserRepository _userRepository;

        private readonly IEmailService _emailService;
        private readonly ITransactionLogger _transactionLogger;

        public List<Transaction> GetTransactionHistory(int clientId)
        {
            return _transactionLogger.GetByClientId(clientId);
        }
        public AtmService(IUserRepository userRepository, IEmailService emailService, ITransactionLogger transactionLogger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _transactionLogger = transactionLogger;
        }


        public List<Client> GetAllClients()
        {
            List<User> users = _userRepository.GetAllUsers();
            return users.OfType<Client>().ToList();
        }

        public bool SetClientBlockedStatus(int clientId, bool isBlocked)
        {
            List<User> users = _userRepository.GetAllUsers();
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null) return false;

            client.IsBlocked = isBlocked;
            _userRepository.SaveUsers(users);
            return true;
        }

        public bool Deposit(int clientId, decimal amount)
        {
            if (amount <= 0) return false;

            List<User> users = _userRepository.GetAllUsers();
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null) return false;

            client.Balance += amount;
            _userRepository.SaveUsers(users);
            _transactionLogger.LogTransaction(client.Id, "Deposit", amount, client.Balance);
            return true;
        }

        public bool Withdraw(int clientId, decimal amount)
        {
            if (amount <= 0) return false;

            List<User> users = _userRepository.GetAllUsers();
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null || client.Balance < amount) return false;

            client.Balance -= amount;
            _userRepository.SaveUsers(users);
            _transactionLogger .LogTransaction(client.Id, "Withdraw", amount, client.Balance);
            return true;
        }

        public bool RequestLoan(int clientId, decimal amount, decimal salary)
        {
            if (salary <= 0 || amount <= 0)
                return false;

            List<User> users = _userRepository.GetAllUsers();
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null) return false;

            client.Salary = salary;
            client.LoadStatus = "Pending";
            client.LoanAmount = amount;

            _userRepository.SaveUsers(users);
            return true;
        }
        public bool TransferMoney(int senderId, string recipientIdentifier, decimal amount)
        {
            if (amount <= 0) return false;

            List<User> users = _userRepository.GetAllUsers();
            Client sender = users.OfType<Client>().FirstOrDefault(c => c.Id == senderId);

            if (sender == null || sender.Balance < amount) return false;

            Client recipient = users.OfType<Client>().FirstOrDefault(c =>
                c.Id.ToString() == recipientIdentifier ||
                c.Email.Equals(recipientIdentifier, StringComparison.OrdinalIgnoreCase));

            if (recipient == null || recipient.Id == sender.Id) return false;

            sender.Balance -= amount;
            recipient.Balance += amount;

            _userRepository.SaveUsers(users);

            _transactionLogger.LogTransaction(sender.Id, "Transfer Out", amount, sender.Balance);
            _transactionLogger.LogTransaction(recipient.Id, "Transfer In", amount, recipient.Balance);

            return true;
        }

        public List<Client> GetPendingLoans()
        {
            List<User> users = _userRepository.GetAllUsers();
            return users.OfType<Client>().Where(c => c.LoadStatus == "Pending").ToList();
        }

        public bool ProcessLoan(int clientId, bool isApproved)
        {
            List<User> users = _userRepository.GetAllUsers();
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null || client.LoadStatus != "Pending") return false;

            if (isApproved)
            {
                client.LoadStatus = "Approved";
                client.Balance += client.LoanAmount;
                _emailService.SendLoanApprovalEmail(client.Email, client.UserName, client.LoanAmount);
            }
            else
            {
                client.LoadStatus = "Rejected";
                _emailService.SendLoanRejectionEmail(client.Email, client.UserName, client.LoanAmount);
            }

            _userRepository.SaveUsers(users);
            return true;
        }

        public Client GetClientById(int clientId)
        {
            List<User> users = _userRepository.GetAllUsers();
            return users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);
        }
    }
}