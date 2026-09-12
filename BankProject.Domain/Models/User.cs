using System.Text.Json.Serialization;

namespace BankProject.Domain.Models
{
    [JsonDerivedType(typeof(Client), typeDiscriminator: "Client")]
    [JsonDerivedType(typeof(Admin), typeDiscriminator: "Admin")]
    public abstract class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        protected User(int id, string userName, string password, string email, string role)
        {
            Id = id;
            UserName = userName;
            Password = password;
            Email = email;
            Role = role;
        }
    }
}