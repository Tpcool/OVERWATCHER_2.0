using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class CurrencyCommands : ModuleBase<SocketCommandContext>
    {
        private Random _rndm = new Random();
        private const string commandType = "currency";

        [Command("getbux"), Remarks(commandType),
            Summary("Displays how much currency you have, or of the user specified.")]
        public async Task Getbux([Remainder]string user = "")
        {
            string currency = Messages.GetAlert("Currency");
            if (Functions.HasMentionedUsers(Context.Message))
            {
                await Context.Channel.SendMessageAsync(Messages.GetAlert("Command.Ping"));
                return;
            }
            SocketUser mentionedUser = Functions.GetUserWithSimilarName(user);
            SocketUser target = mentionedUser ?? Context.User;
            var account = UserAccounts.GetAccount(target);

            await Context.Channel.SendMessageAsync($"`{target.Username.ToLower()}` has `{account.GetFormattedCurrency()}` {currency}.");
        }

        [Command("doubleornothing"), Remarks(commandType),
            Summary("A 1/2 chance of doubling your currency. If it fails, your currency is reset.")]
        public async Task DoubleOrNothing([Remainder]string nothing = "")
        {
            SocketUser user = Context.User;
            UserAccount userAccount = UserAccounts.GetAccount(user);
            string msg, currency = Messages.GetAlert("Currency");

            if (_rndm.Next(2) == 0)
            {
                userAccount.Currency *= 2;
                msg = $"you have successfully doubled your {currency.ToUpper()}! ";
                msg += Messages.GetAlert("Currency.Has(user, currency, cName)", user.Username.ToLower(), userAccount.GetFormattedCurrency(), currency);
            }
            else
            {
                userAccount.Currency = 0;
                msg = Messages.GetAlert("Currency.Lose");
            }

            Currency.TryRevertImaginary(userAccount);
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("tripleornothing"), Remarks(commandType),
            Summary("A 1/3 chance of tripling your currency. If it fails, your currency is reset.")]
        public async Task TripleOrNothing([Remainder]string nothing = "")
        {
            SocketUser user = Context.User;
            UserAccount userAccount = UserAccounts.GetAccount(user);
            string msg, currency = Messages.GetAlert("Currency");

            if (_rndm.Next(3) == 0)
            {
                userAccount.Currency *= 3;
                msg = $"you have successfully TRIPPLED your {currency.ToUpper()}!!!";
                msg += Messages.GetAlert("Currency.Has(user, currency, cName)", user.Username.ToLower(), userAccount.GetFormattedCurrency(), currency);
            }
            else
            {
                userAccount.Currency = 0;
                msg = Messages.GetAlert("Currency.Lose");
            }

            Currency.TryRevertImaginary(userAccount);
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("sans"), Remarks(commandType),
            Summary("Posts a pornographic image of Sans from Undertale.")]
        public async Task Sans([Remainder]string nothing = "")
        {
            // Change to shortened link
            // https://dev.bitly.com/get_started.html
            UserAccount user = UserAccounts.GetAccount(Context.User);
            int cost = Constants.SansCost;
            if (user.Currency < cost)
            {
                await Context.Channel.SendMessageAsync($"not enough money dumbass you need {cost}.");
                return;
            }
            else
            {
                user.AddCurrency(-200);
                Currency.TryRevertImaginary(user);
            }

            user.GetFormattedCurrency();
            string json, imageUrl, tags;
            tags = Functions.GetFormattedTagParams("sans_(undertale)", "rating:e", "type:jpg", "type:png", "order:random&limit=1");
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "Tpcool (thomas@polese.us)");
                json = client.DownloadString(Messages.GetAlert("Api.Sans(tags)", tags));
            }
            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);
            imageUrl = dataObject[0].file_url.ToString();
            await Context.Channel.SendMessageAsync(imageUrl);
        }
    }
}
