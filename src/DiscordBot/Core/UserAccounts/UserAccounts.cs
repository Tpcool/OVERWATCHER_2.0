using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Core
{
    public static class UserAccounts
    {
        private static List<UserAccount> accounts;
        private static readonly string path = @"..\..\..\SystemLang\accounts.json";

        static UserAccounts()
        {
            if (DataStorage.SaveExists(path))
            {
                accounts = DataStorage.LoadUserAccounts(path).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, path);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
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
                Ravebux = 0,
                IsImaginary = false
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
