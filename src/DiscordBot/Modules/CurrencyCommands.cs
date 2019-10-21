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
            // TODO: Make post tags easier to read/change
            // Improve temp file system
            // Implement currency that must be spent to invoke command
            string json = "";
            string fileName = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "Overwatcher (Discord bot by Tpcool thomas@polese.us)");
                json = client.DownloadString("https://e621.net/post/index.json?tags=sans_(undertale)+rating:e+type:jpg+type:png+order:random&limit=1");
                var dataObject = JsonConvert.DeserializeObject<dynamic>(json);
                string imageUrl = dataObject[0].file_url.ToString();
                string imageExt = "." + dataObject[0].file_ext.ToString();
                fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + imageExt;
                client.DownloadFile(imageUrl, fileName);
            }

            await Context.Channel.SendFileAsync(fileName);
        }
    }
}
