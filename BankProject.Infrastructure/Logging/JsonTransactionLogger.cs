using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;

namespace BankProject.Infrastructure.Logging
{
    public class JsonTransactionLogger : ITransactionLogger
    {
        private readonly string _filePath;
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public JsonTransactionLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void LogTransaction(int clientId, string type, decimal amount, decimal balanceAfter)
        {
            try
            {
                List<Transaction> transactions = LoadAll();

                transactions.Add(new Transaction
                {
                    ClientId = clientId,
                    Type = type,
                    Amount = amount,
                    BalanceAfter = balanceAfter,
                    Date = DateTime.Now
                });

                File.WriteAllText(_filePath, JsonSerializer.Serialize(transactions, _options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Transaction Log Error] {ex.Message}");
            }
        }

        public List<Transaction> GetByClientId(int clientId)
        {
            return LoadAll()
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.Date)
                .ToList();
        }

        private List<Transaction> LoadAll()
        {
            if (!File.Exists(_filePath))
                return new List<Transaction>();

            try
            {
                string json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Transaction>();

                return JsonSerializer.Deserialize<List<Transaction>>(json, _options) ?? new List<Transaction>();
            }
            catch
            {
                return new List<Transaction>();
            }
        }
    }
}