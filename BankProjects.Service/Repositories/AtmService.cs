using System;
using System.Collections.Generic;
using System.Linq;
using BankProject.Domain.Models;
using BankProject.Infrastructure.Repositories;

namespace BankProjects.Service
{
    public class AtmService
    {
        private readonly string _filePath;

        public AtmService(string filePath)
        {
            _filePath = filePath;
        }

        public bool Deposit(int clientId, decimal amount)
        {
            if (amount <= 0) return false;

            List<User> users = FileManager.LoadUsersFromFile(_filePath);
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null) return false;

            client.Balance += amount;
            FileManager.SaveUsersToFile(users, _filePath);
            return true;
        }

        public bool Withdraw(int clientId, decimal amount)
        {
            if (amount <= 0) return false;

            List<User> users = FileManager.LoadUsersFromFile(_filePath);
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null || client.Balance < amount) return false;

            client.Balance -= amount;
            FileManager.SaveUsersToFile(users, _filePath);
            return true;
        }

        public bool RequestLoan(int clientId, decimal amount)
        {
            if (amount <= 0) return false;

            List<User> users = FileManager.LoadUsersFromFile(_filePath);
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null) return false;

            client.LoadStatus = "Pending";
            client.LoanAmount = amount;

            FileManager.SaveUsersToFile(users, _filePath);
            return true;
        }

        public List<Client> GetPendingLoans()
        {
            List<User> users = FileManager.LoadUsersFromFile(_filePath);
            return users.OfType<Client>().Where(c => c.LoadStatus == "Pending").ToList();
        }

        public bool ProcessLoan(int clientId, bool isApproved)
        {
            List<User> users = FileManager.LoadUsersFromFile(_filePath);
            Client client = users.OfType<Client>().FirstOrDefault(c => c.Id == clientId);

            if (client == null || client.LoadStatus != "Pending") return false;

            if (isApproved)
            {
                client.LoadStatus = "Approved";
                client.Balance += client.LoanAmount;
            }
            else
            {
                client.LoadStatus = "Rejected";
            }

            FileManager.SaveUsersToFile(users, _filePath);
            return true;
        }
    }
}