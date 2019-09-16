using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Core
{
    public static class UserAccounts
    {
        private static List<UserAccount> Accounts;
        private static readonly string Path = @"..\..\..\SystemLang\accounts.json";

        public static List<UserAccount> GetAccounts() => Accounts;

        static UserAccounts()
        {
            if (DataStorage.SaveExists(Path))
            {
                Accounts = DataStorage.LoadUserAccounts(Path).ToList();
            }
            else
            {
                Accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(Accounts, Path);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in Accounts
                         where a.Id == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                Id = id,
                Currency = 0,
                IsCurrencyImaginary = false
            };
            
            Accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
