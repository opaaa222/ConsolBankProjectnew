using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Service.Interfaces;
using System;

namespace BankProject.Ui
{
    internal class AuthMenue
    {
        private readonly IAuthService _authService;
        private readonly IAtmService _atmService;
        private readonly ILoginLogger _loginLogger;

        public AuthMenue(IAuthService authService, IAtmService atmService, ILoginLogger loginLogger)
        {
            _authService = authService;
            _atmService = atmService;
            _loginLogger = loginLogger;
        }

        public void RegisterMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Registration ===");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            try
            {
                Client newClient = _authService.RegisterClient(username, password, email);
                Console.WriteLine($"\nRegistration completed successfully! Your unique ID is: {newClient.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nRegistration error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void LoginMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Login ===");
            Console.Write("Enter Email or Username: ");
            string input = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            User user = _authService.Login(input, password);

            if (user == null)
            {
                Console.WriteLine("\nInvalid credentials! Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nWelcome, {user.UserName}!");
            Console.ReadKey();

            if (user is Client client)
            {
                ClientMenu clientMenu = new ClientMenu(_atmService);
                clientMenu.Show(client);
            }
            else if (user is Admin admin)
            {
                AdminMenu adminMenu = new AdminMenu(_atmService, _loginLogger);
                adminMenu.Show(admin);
            }
        }
    }
}