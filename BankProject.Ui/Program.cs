using System;
using System.Collections.Generic;
using BankProject.Domain.Models;
using BankProjects.Service;

namespace BankProject.Ui
{
    internal class Program
    {
        private static readonly string filePath = "users.json";
        private static AuthService authService;
        private static AtmService atmService;

        static void Main(string[] args)
        {

            authService = new AuthService(filePath);
            atmService = new AtmService(filePath);

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
                        LoginMenu();
                        break;

                    case "2":
                        RegisterMenu();
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
        }

        #region Login and Registration

        static void LoginMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Login ===");
            Console.Write("Enter Email or Username: ");
            string input = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            User user = authService.Login(input, password);

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
                ShowClientMenu(client);
            }
            else if (user is Admin admin)
            {
                ShowAdminMenu(admin);
            }
        }

        static void RegisterMenu()
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
                Client newClient = authService.RegisterClient(username, password, email);
                Console.WriteLine($"\nRegistration completed successfully! Your unique ID is: {newClient.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nRegistration error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        #endregion

        #region Client Menu

        static void ShowClientMenu(Client client)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Client Menu ({client.UserName}) ===");
                Console.WriteLine("1. Check Balance");
                Console.WriteLine("2. Deposit Money");
                Console.WriteLine("3. Withdraw Money");
                Console.WriteLine("4. Request a Loan");
                Console.WriteLine("0. Logout");
                Console.WriteLine("=============================");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine($"\nCurrent Balance: {client.Balance} GEL");
                        Console.WriteLine($"Loan Status: {client.LoadStatus} (Amount: {client.LoanAmount} GEL)");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("\nEnter deposit amount: ");

                        if (decimal.TryParse(Console.ReadLine(), out decimal depAmount) && depAmount > 0)
                        {
                            if (atmService.Deposit(client.Id, depAmount))
                            {
                                client.Balance += depAmount;
                                Console.WriteLine("Money deposited successfully!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount!");
                        }

                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Write("\nEnter withdrawal amount: ");

                        if (decimal.TryParse(Console.ReadLine(), out decimal withAmount) && withAmount > 0)
                        {
                            if (atmService.Withdraw(client.Id, withAmount))
                            {
                                client.Balance -= withAmount;
                                Console.WriteLine("Money withdrawn successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Operation failed (insufficient balance)!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount!");
                        }

                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Write("\nEnter desired loan amount: ");

                        if (decimal.TryParse(Console.ReadLine(), out decimal loanAmount) && loanAmount > 0)
                        {
                            if (atmService.RequestLoan(client.Id, loanAmount))
                            {
                                client.LoadStatus = "Pending";
                                client.LoanAmount = loanAmount;
                                Console.WriteLine("Loan request has been sent to the administrator!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount!");
                        }

                        Console.ReadKey();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        #endregion

        #region Admin Menu

        static void ShowAdminMenu(Admin admin)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Admin Menu ({admin.UserName}) ===");
                Console.WriteLine("1. Review Loan Requests");
                Console.WriteLine("0. Logout");
                Console.WriteLine("=================================");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    List<Client> pendingLoans = atmService.GetPendingLoans();

                    if (pendingLoans.Count == 0)
                    {
                        Console.WriteLine("\nThere are no pending loan requests.");
                        Console.ReadKey();
                        continue;
                    }

                    Console.WriteLine("\n--- Pending Loans ---");

                    foreach (var c in pendingLoans)
                    {
                        Console.WriteLine($"ID: {c.Id} | Client: {c.UserName} | Requested Amount: {c.LoanAmount} GEL | balans: {c.Balance} GEL");
                    }

                    Console.Write("\nEnter the client ID to process the loan: ");

                    if (int.TryParse(Console.ReadLine(), out int selectedId))
                    {
                        Console.Write("Approve the loan? (1 - Approve / 0 - Reject): ");
                        string decision = Console.ReadLine();

                        bool isApproved = decision == "1";

                        if (atmService.ProcessLoan(selectedId, isApproved))
                        {
                            Console.WriteLine("Loan status updated successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Client not found or loan status is not Pending!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID!");
                    }

                    Console.ReadKey();
                }
                else if (choice == "0")
                {
                    return;
                }
            }
        }


        static void SeedAdmin()
        {
            var users = BankProject.Infrastructure.Repositories.FileManager.LoadUsersFromFile(filePath);

            
            if (!users.Any(u => u.Role == "Admin"))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
                Admin admin = new Admin(999999, "admin", hashedPassword, "admin@bank.ge");
                users.Add(admin);
                BankProject.Infrastructure.Repositories.FileManager.SaveUsersToFile(users, filePath);
            }
        }




        #endregion
    }
}

