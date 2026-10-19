using System.Collections.Generic;
using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;

namespace BankProject.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {    
        private readonly string _filePath;
       
        public UserRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<User> GetAllUsers()
        {
            return FileManager.LoadUsersFromFile(_filePath);
        }

        public void SaveUsers(List<User> users)
        {
            FileManager.SaveUsersToFile(users, _filePath);
        }
    }
}
