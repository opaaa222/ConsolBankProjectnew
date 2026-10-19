using BankProject.Domain.Models;

namespace BankProject.Service.Interfaces
{
    public interface IAuthService
    {
        bool IsValidEmail(string email);
        User Login(string emailOrUsername, string password);
        Client RegisterClient(string username, string password, string email, decimal initialBalance = 0);
    }
}
