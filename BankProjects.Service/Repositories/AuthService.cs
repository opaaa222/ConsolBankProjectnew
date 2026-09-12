using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BankProject.Domain.Models;
using BankProject.Infrastructure.Repositories;
using BCrypt.Net;

namespace BankProjects.Service
{
    public class AuthService
    {
        private readonly string _filePath;

        public AuthService(string filePath)
        {
            _filePath = filePath;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public Client RegisterClient(string username, string password, string email, decimal initialBalance = 0)
        {
            if (!IsValidEmail(email))
            {
                throw new Exception("The entered email format is invalid!");
            }

            List<User> users = FileManager.LoadUsersFromFile(_filePath);

            if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("A user with this email already exists!");
            }

            int newId = IdGeneratorService.GenerateUniqueId(users);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            Client newClient = new Client(
                newId,
                username,
                hashedPassword,
                email,
                "Client",
                initialBalance,
                "None",
                0
            );

            users.Add(newClient);
            FileManager.SaveUsersToFile(users, _filePath);

            return newClient;
        }

        public User Login(string emailOrUsername, string password)
        {
            List<User> users = FileManager.LoadUsersFromFile(_filePath);

            User user = users.FirstOrDefault(u =>
                u.Email.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                u.UserName.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return isPasswordCorrect ? user : null;
        }
    }
}
