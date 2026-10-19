using System.Text.Json.Serialization;

namespace BankProject.Domain.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Client), typeDiscriminator: "client")]
    [JsonDerivedType(typeof(Admin), typeDiscriminator: "admin")]
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