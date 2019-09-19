using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord.WebSocket;

namespace DiscordBot.Core
{
    /// <summary>
    /// Handles the currency system of the bot.
    /// </summary>
    public static class Currency
    {
        /// <summary>
        /// Attempts to locate the currency of the user given using only a string of their name instead of their account.
        /// If there are no matches, it will return a null dictionary.
        /// </summary>
        public static Dictionary<string, int> GetUserCurrency(string testName)
        {
            var userCurrency = new Dictionary<string, int>();
            var user = Functions.GetUserWithSimilarName(testName);

            if (user != null) userCurrency.Add(user.Username, UserAccounts.GetAccount(user).Currency);
            else userCurrency = null;
            
            return userCurrency;
        }

        /// <summary>
        /// Transfers currency between two user accounts.
        /// </summary>
        /// <param name="amount">Must be a positive number.</param>
        public static void MoveCurrency(UserAccount accountToGive, UserAccount accountToTake, int amount)
        {
            if (amount < 1)
            {
                Console.WriteLine("An invalid amount of currency attempted to be transfered. Returning...");
                return;
            }
            accountToGive.AddCurrency(amount * -1);
            accountToTake.AddCurrency(amount);
        }

        /// <summary>
        /// If the user's balance meets the conditions for losing its imaginary status, it will revert.
        /// </summary>
        public static void TryRevertImaginary(UserAccount acct)
        {
            if (acct.IsCurrencyImaginary && acct.Currency <= 0) acct.IsCurrencyImaginary = false;
        }
    }
}
