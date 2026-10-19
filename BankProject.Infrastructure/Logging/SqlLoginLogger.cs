using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankProject.Infrastructure.Logging
{
    public class SqlLoginLogger : ILoginLogger
    {
        public void LogLogin(string userName)
        {
            try
            {
                using var context = new BankDbContext();

                context.LoginEntries.Add(new LoginEntry
                {
                    UserName = userName,
                    IpAddress = GetLocalIPAddress(),
                    LoginTime = DateTime.Now
                });

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Log Error] {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public List<LoginEntry> GetAll()
        {
            try
            {
                using var context = new BankDbContext();
                return context.LoginEntries
                    .AsNoTracking()
                    .OrderByDescending(e => e.LoginTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Log Error] {ex.InnerException?.Message ?? ex.Message}");
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