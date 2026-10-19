using System;
using System.Collections.Generic;
using System.Text;

namespace BankProject.Domain.Models
{
    public class Admin : User
    {
        public Admin(int id, string userName, string password, string email )
            : base(id, userName, password, email, "Admin")
        {
        }

       
    
    }
}
    