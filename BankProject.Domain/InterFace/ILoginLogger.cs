using System.Collections.Generic;
using BankProject.Domain.Models;

namespace BankProject.Domain.Interfaces
{
    public interface ILoginLogger
    {
        void LogLogin(string userName);
        List<LoginEntry> GetAll();
    }
}