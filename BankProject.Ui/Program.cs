using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Infrastructure;
using BankProject.Infrastructure.Data;
using BankProject.Infrastructure.Logging;
using BankProject.Infrastructure.Repositories;
using BankProject.Service.Interfaces;
using BankProject.Ui;
using BankProjects.Service.implementation;
using BankProjects.Service.interfaces;
using System;
using System.Linq;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool useSql = true;   

IUserRepository userRepository;
ILoginLogger loginLogger;
ITransactionLogger transactionLogger;

if (useSql)
{
    using (var db = new BankDbContext())
    {
        db.Database.EnsureCreated();
    }

    userRepository = new SqlUserRepository();
    loginLogger = new SqlLoginLogger();
    transactionLogger = new SqlTransactionLogger();
}
else
{
    userRepository = new UserRepository(AppPaths.UsersFilePath);
    loginLogger = new JsonLoginLogger(AppPaths.LoginLogFilePath);
    transactionLogger = new JsonTransactionLogger(AppPaths.TransactionsFilePath);
}

IEmailService emailService = new EmailService();

IAuthService authService = new AuthService(userRepository, loginLogger);
IAtmService atmService = new AtmService(userRepository, emailService, transactionLogger);

AuthMenue authMenu = new AuthMenue(authService, atmService, loginLogger);

SeedAdmin();

while (true)
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("       ATM Simulator             ");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Login");
    Console.WriteLine("2. Register");
    Console.WriteLine("0. Exit");
    Console.WriteLine("=================================");
    Console.Write("Select an option: ");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            authMenu.LoginMenu();
            break;
        case "2":
            authMenu.RegisterMenu();
            break;
        case "0":
            Console.WriteLine("\nThank you for using our ATM!");
            return;
        default:
            Console.WriteLine("\nInvalid choice! Press any key to continue...");
            Console.ReadKey();
            break;
    }
}

void SeedAdmin()
{
    var users = userRepository.GetAllUsers();

    if (!users.Any(u => u.Role == "Admin"))
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
        Admin admin = new Admin(999999, "admin", hashedPassword, "admin@bank.ge");
        users.Add(admin);
        userRepository.SaveUsers(users);
    }
}