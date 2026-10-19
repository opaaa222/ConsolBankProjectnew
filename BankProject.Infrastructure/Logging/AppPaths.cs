using System;
using System.IO;

namespace BankProject.Infrastructure
{
    public static class AppPaths
    {
        private static readonly string _dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");

        public static string UsersFilePath => Path.Combine(_dataFolder, "users.json");
        public static string LoginLogFilePath => Path.Combine(_dataFolder, "login_log.json");
        public static string TransactionsFilePath => Path.Combine(_dataFolder, "transactions.json");

        static AppPaths()
        {
            if (!Directory.Exists(_dataFolder))
                Directory.CreateDirectory(_dataFolder);
        }
    }
}