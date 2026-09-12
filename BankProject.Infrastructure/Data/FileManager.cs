using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BankProject.Domain.Models; 

namespace BankProject.Infrastructure.Repositories
{
    public static class FileManager
    {
       
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

      
        public static void SaveUsersToFile(List<User> users, string filePath)
        {
            try
            {
               
                string jsonString = JsonSerializer.Serialize(users, _options);
                
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"shenaxvisas shecdoma {ex.Message}");
            }
        }

       public static List<User> LoadUsersFromFile(string filePath)
        {
            var users = new List<User>();

            if (!File.Exists(filePath))
                return users;

            try
            {
              
                string jsonString = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(jsonString))
                    return users;

               
                users = JsonSerializer.Deserialize<List<User>>(jsonString, _options) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"shecdoma wakitxvisas {ex.Message}");
            }

            return users;
        }
    }
}