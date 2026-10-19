using System.Collections.Generic;
using System.Linq;
using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankProject.Infrastructure.Repositories
{
    public class SqlUserRepository : IUserRepository
    {
        public List<User> GetAllUsers()
        {
            using var context = new BankDbContext();
            return context.Users.AsNoTracking().ToList();
        }

        public void SaveUsers(List<User> users)
        {
            using var context = new BankDbContext();
            var existingIds = context.Users.Select(u => u.Id).ToHashSet();

            foreach (var user in users)
            {
                if (existingIds.Contains(user.Id))
                    context.Users.Update(user);
                else
                    context.Users.Add(user);
            }

            context.SaveChanges();
        }
    }
}