using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class CurrencyCommands : ModuleBase<SocketCommandContext>
    {
        private Random _rndm = new Random();

        [Command("getbux"), Remarks("currency"),
            Summary("Displays how much currency you have, or of the user specified.")]
        public async Task Getbux([Remainder]string user = "")
        {
            if (Functions.HasMentionedUsers(Context.Message))
            {
                await Context.Channel.SendMessageAsync("don't ping people asshole. just type their username.");
                return;
            }
            SocketUser mentionedUser = Functions.GetUserWithSimilarName(user);
            SocketUser target = mentionedUser ?? Context.User;
            var account = UserAccounts.GetAccount(target);

            await Context.Channel.SendMessageAsync($"`{target.Username.ToLower()}` has `{account.GetFormattedCurrency()}` ravebux.");
        }

        [Command("doubleornothing"), Remarks("currency"),
            Summary("A 1/2 chance of doubling your currency. If it fails, your currency is reset.")]
        public async Task DoubleOrNothing([Remainder]string nothing = "")
        {
            SocketUser user = Context.User;
            UserAccount userAccount = UserAccounts.GetAccount(user);
            string msg;

            if (_rndm.Next(2) == 0)
            {
                userAccount.Currency *= 2;
                msg = $"you have successfully doubled your RAVEBUX! `{user.Username}` now has `{userAccount.GetFormattedCurrency()}` ravebux.";
            }
            else
            {
                userAccount.Currency = 0;
                msg = "you lost all your cash lmao";
            }

            Currency.TryRevertImaginary(userAccount);
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("tripleornothing"), Remarks("currency"),
            Summary("A 1/3 chance of tripling your currency. If it fails, your currency is reset.")]
        public async Task TripleOrNothing([Remainder]string nothing = "")
        {
            SocketUser user = Context.User;
            UserAccount userAccount = UserAccounts.GetAccount(user);
            string msg;

            if (_rndm.Next(3) == 0)
            {
                userAccount.Currency *= 3;
                msg = $"you have successfully TRIPPLED your RAVEBUX!!! `{user.Username}` now has `{userAccount.GetFormattedCurrency()}` ravebux.";
            }
            else
            {
                userAccount.Currency = 0;
                msg = "you lost all your cash lmao";
            }

            Currency.TryRevertImaginary(userAccount);
            await Context.Channel.SendMessageAsync(msg);
        }
    }
}
