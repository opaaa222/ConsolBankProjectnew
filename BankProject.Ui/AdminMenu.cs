using BankProject.Domain.Interfaces;
using BankProject.Domain.Models;
using BankProject.Service.Interfaces;
using System;
using System.Collections.Generic;


namespace BankProject.Ui
{
    public class AdminMenu
    {
        private readonly IAtmService _atmService;
        private readonly IUserRepository userRepository;
        private readonly ILoginLogger _loginLogger;

        public AdminMenu(IAtmService atmService, ILoginLogger loginLogger)
        {
            _atmService = atmService;
            _loginLogger = loginLogger;
        }

        //public AdminMenu(IAtmService atmService, IUserRepository userRepository, ILoginLogger loginLogger)
        //{
        //    _atmService = atmService;
        //    this.userRepository = userRepository;
        //    this._loginLogger = loginLogger;
        //}

        public void Show(Admin admin)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Admin Menu ({admin.UserName}) ===");
                Console.WriteLine("1. Review Loan Requests");
                Console.WriteLine("2. View All Clients");
                Console.WriteLine("3. Block/Unblock a Client");
                Console.WriteLine("4. View Login History");
                Console.WriteLine("0. Logout");
                Console.WriteLine("=================================");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    List<Client> pendingLoans = _atmService.GetPendingLoans();

                    if (pendingLoans.Count == 0)
                    {
                        Console.WriteLine("\nThere are no pending loan requests.");
                        Console.ReadKey();
                        continue;
                    }

                    Console.WriteLine("\n--- Pending Loans ---");
                    foreach (var c in pendingLoans)
                    {
                        Console.WriteLine($"ID: {c.Id} | Client: {c.UserName} | Requested Amount: {c.LoanAmount} GEL | Balance: {c.Balance} GEL | Salary: {c.Salary} GEL");
                    }

                    Console.Write("\nEnter the client ID to process the loan: ");
                    if (int.TryParse(Console.ReadLine(), out int selectedId))
                    {
                        Console.Write("Approve the loan? (1 - Approve / 0 - Reject): ");
                        string decision = Console.ReadLine();
                        bool isApproved = decision == "1";

                        if (_atmService.ProcessLoan(selectedId, isApproved))
                            Console.WriteLine("Loan status updated successfully!");
                        else Console.WriteLine("Client not found or loan status is not Pending!");
                    }
                    else Console.WriteLine("Invalid ID!");

                    Console.ReadKey();
                }
                else if (choice == "2")
                {
                    List<Client> allClients = _atmService.GetAllClients();

                    if (allClients.Count == 0)
                    {
                        Console.WriteLine("\nNo clients registered yet.");
                        Console.ReadKey();
                        continue;
                    }

                    Console.WriteLine("\n--- All Clients ---");
                    foreach (var c in allClients)
                    {
                        string status = c.IsBlocked ? "BLOCKED" : "Active";
                        Console.WriteLine($"ID: {c.Id} | Name: {c.UserName} | Email: {c.Email} | Balance: {c.Balance} GEL | Status: {status}");
                    }
                    Console.ReadKey();
                }
                else if (choice == "3")
                {
                    Console.Write("\nEnter the client ID to block/unblock: ");
                    if (int.TryParse(Console.ReadLine(), out int clientId))
                    {
                        Console.Write("1 - Block / 0 - Unblock: ");
                        string decision = Console.ReadLine();
                        bool block = decision == "1";

                        if (_atmService.SetClientBlockedStatus(clientId, block))
                            Console.WriteLine(block ? "Client has been blocked." : "Client has been unblocked.");
                        else
                            Console.WriteLine("Client not found!");
                    }
                    else Console.WriteLine("Invalid ID!");

                    Console.ReadKey();
                }
                else if (choice == "4")
                {
                    var loginHistory = _loginLogger.GetAll();

                    if (loginHistory.Count == 0)
                    {
                        Console.WriteLine("\nNo login history found.");
                        Console.ReadKey();
                        continue;
                    }

                    Console.WriteLine("\n--- Login History ---");
                    Console.WriteLine($"{"Date/Time",-20} | {"User",-15} | {"IP Address",-15}");
                    Console.WriteLine(new string('-', 55));

                    foreach (var entry in loginHistory.OrderByDescending(e => e.LoginTime))
                    {
                        Console.WriteLine($"{entry.LoginTime:yyyy-MM-dd HH:mm:ss,-20} | {entry.UserName,-15} | {entry.IpAddress,-15}");
                    }

                    Console.ReadKey();
                }


                else if (choice == "0")
                {
                    return;
                }
            }
        }

        internal void SeedAdmin()
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
    }
}