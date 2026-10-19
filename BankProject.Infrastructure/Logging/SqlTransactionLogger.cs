using System;
using System.Collections.Generic;
using System.Linq;
using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankProject.Infrastructure.Logging
{
    public class SqlTransactionLogger : ITransactionLogger
    {
        public void LogTransaction(int clientId, string type, decimal amount, decimal balanceAfter)
        {
            try
            {
                using var context = new BankDbContext();

                context.Transactions.Add(new Transaction
                {
                    ClientId = clientId,
                    Type = type,
                    Amount = amount,
                    BalanceAfter = balanceAfter,
                    Date = DateTime.Now
                });

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Transaction Log Error] {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public List<Transaction> GetByClientId(int clientId)
        {
            using var context = new BankDbContext();
            return context.Transactions
                .AsNoTracking()
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.Date)
                .ToList();
        }
    }
}