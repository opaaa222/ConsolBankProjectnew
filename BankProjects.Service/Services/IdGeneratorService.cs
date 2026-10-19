using System;
using System.Collections.Generic;
using System.Linq;
using BankProject.Domain.Models;

namespace BankProjects.Service.Services
{
    public static class IdGeneratorService
    {
        private static readonly Random _random = new Random();

        public static int GenerateUniqueId(List<User> existingUsers)
        {
            int newId;
            do
            {
                newId = _random.Next(100000, 1000000);
            }
            while (existingUsers.Any(u => u.Id == newId));

            return newId;
        }
    }
}