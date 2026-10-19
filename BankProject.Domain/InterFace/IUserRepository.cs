using System.Collections.Generic;
using BankProject.Domain.Models;

namespace BankProject.Domain.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        void SaveUsers(List<User> users);
    }
}
