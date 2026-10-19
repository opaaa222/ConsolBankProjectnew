
using BankProject.Domain.Models;
using BankProject.Service.Interfaces;

namespace BankProject.Ui
{
    public class ClientMenu
    {
        private readonly IAtmService _atmService;

        public ClientMenu(IAtmService atmService)
        {
            _atmService = atmService;
        }

        public void Show(Client client)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Client Menu ({client.UserName}) ({client.Id}) id ===");
                Console.WriteLine("1. Check Balance");
                Console.WriteLine("2. Deposit Money");
                Console.WriteLine("3. Withdraw Money");
                Console.WriteLine("4. Request a Loan");
                Console.WriteLine("5. View Transaction History");
                Console.WriteLine("6. Transfer Money");
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
                            if (_atmService.Deposit(client.Id, depAmount))
                            {
                                client = _atmService.GetClientById(client.Id);
                                Console.WriteLine("Money deposited successfully!");
                            }
                            else Console.WriteLine("Deposit failed!");
                        }
                        else Console.WriteLine("Invalid amount!");
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.Write("\nEnter withdrawal amount: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withAmount) && withAmount > 0)
                        {
                            if (_atmService.Withdraw(client.Id, withAmount))
                            {
                                client = _atmService.GetClientById(client.Id);
                                Console.WriteLine("Money withdrawn successfully!");
                            }
                            else Console.WriteLine("Operation failed (insufficient balance)!");
                        }
                        else Console.WriteLine("Invalid amount!");
                        Console.ReadKey();
                        break;
                    case "4":
                        Console.Write("\nEnter your monthly salary: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal salary) && salary > 0)
                        {
                            Console.Write("Enter the desired loan amount: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal loanAmount) && loanAmount > 0)
                            {
                                if (_atmService.RequestLoan(client.Id, loanAmount, salary))
                                {
                                    client = _atmService.GetClientById(client.Id);
                                    Console.WriteLine("Your loan request has been sent to the administrator!");
                                }
                                else Console.WriteLine("Your loan request was rejected.");
                            }
                        }
                        else Console.WriteLine("A valid salary is required!");
                        Console.ReadKey();
                        break;
                    case "5":
                        var history = _atmService.GetTransactionHistory(client.Id);
                        Console.WriteLine("\n--- Transaction History ---");
                        if (history.Count == 0)
                        {
                            Console.WriteLine("No transactions yet.");
                        }
                        else
                        {
                            foreach (var t in history)
                            {
                                Console.WriteLine($"{t.Date:yyyy-MM-dd HH:mm} | {t.Type} | Amount: {t.Amount} GEL | Balance After: {t.BalanceAfter} GEL");
                            }
                        }
                        Console.ReadKey();
                        break;
                    case "6":
                        Console.Write("\nEnter recipient ID or Email: ");
                        string recipient = Console.ReadLine();
                        Console.Write("Enter amount to transfer: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount) && transferAmount > 0)
                        {
                            if (_atmService.TransferMoney(client.Id, recipient, transferAmount))
                            {
                                client = _atmService.GetClientById(client.Id);
                                Console.WriteLine("Transfer completed successfully!");
                            }
                            else Console.WriteLine("Transfer failed (invalid recipient or insufficient balance)!");
                        }
                        else Console.WriteLine("Invalid amount!");
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
    }
}