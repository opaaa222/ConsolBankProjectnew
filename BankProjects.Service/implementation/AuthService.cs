using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Service.Interfaces;
using BankProjects.Service.Services;
using System.Text.RegularExpressions;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginLogger _loginLogger;
    public AuthService(IUserRepository userRepository, ILoginLogger loginLogger)
    {
        this._userRepository = userRepository;
        this._loginLogger = loginLogger;
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

        List<User> users = _userRepository.GetAllUsers();

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
        _userRepository.SaveUsers(users);

        return newClient;
    }

    public User Login(string emailOrUsername, string password)
    {
        List<User> users = _userRepository.GetAllUsers();

        User user = users.FirstOrDefault(u =>
            u.Email.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase)||
            u.UserName.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase));


        if (user == null)
            return null;

        bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!isPasswordCorrect)
            return null;

        if (user is Client client && client.IsBlocked)
        {
            Console.WriteLine("\nYour account has been blocked by the administrator!");
            return null;
        }

        _loginLogger.LogLogin(user.UserName);
        return user;
    }
}