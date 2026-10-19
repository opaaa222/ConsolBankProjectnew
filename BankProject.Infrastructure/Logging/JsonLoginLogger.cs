using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;

namespace BankProject.Infrastructure.Logging
{
    public class JsonLoginLogger : ILoginLogger
    {
        private readonly string _filePath;
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public JsonLoginLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void LogLogin(string userName)
        {
            try
            {
                List<LoginEntry> entries = GetAll();

                entries.Add(new LoginEntry
                {
                    UserName = userName,
                    IpAddress = GetLocalIPAddress(),
                    LoginTime = DateTime.Now
                });

                File.WriteAllText(_filePath, JsonSerializer.Serialize(entries, _options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Log Error] {ex.Message}");
            }
        }

        public List<LoginEntry> GetAll()
        {
            if (!File.Exists(_filePath))
                return new List<LoginEntry>();

            try
            {
                string json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<LoginEntry>();

                return JsonSerializer.Deserialize<List<LoginEntry>>(json, _options) ?? new List<LoginEntry>();
            }
            catch
            {
                return new List<LoginEntry>();
            }
        }

        private static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}