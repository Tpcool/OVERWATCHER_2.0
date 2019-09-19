using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Utilities;
using NReco.ImageGenerator;

namespace DiscordBot.Core
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("hello", RunMode = RunMode.Async)]
        public async Task Hello([Remainder]string msg)
        {
            var embed = Functions.GetDefaultBotEmbed();
            embed.WithDescription(FormatMessages.GetFormattedAlert("TestMsg_&MSG", msg));
            embed.WithThumbnailUrl(FormatMessages.GetFormattedAlert("TestImg"));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("test", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Test()
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            string role = FormatMessages.GetFormattedAlert("TestRole");
            var embed = Functions.GetDefaultBotEmbed();

            if (Functions.UserHasRole((SocketGuildUser)Context.User, role))
            {
                embed.WithDescription(FormatMessages.GetFormattedAlert("TestRoleMsg_&ROLE", role));
                await dm.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                embed.WithDescription(FormatMessages.GetFormattedAlert("TestNoRoleMsg_&ROLE", role));
                await dm.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("getbux", RunMode = RunMode.Async)]
        public async Task Getbux([Remainder]string user = "")
        {
            SocketUser mentionedUser = Functions.GetUserWithSimilarName(user);
            SocketUser target = mentionedUser ?? Context.User;
            var account = UserAccounts.GetAccount(target);

            var embed = Functions.GetDefaultBotEmbed();
            embed.WithDescription($"{target.Username} has {account.Currency} ravebux. are they imaginary? {account.IsCurrencyImaginary}.");
            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        // Doesn't work??? Can't get HtmlToImageConverter object to instantiate.

        /*
        [Command("image", RunMode = RunMode.Async)]
        public async Task Image([Remainder]string text = "")
        {
            string path = @"..\..\..\index.html";
            string path2 = @"Q:/Desktop/Programming/OVER_W4TCHER 2.0/src/DiscordBot/index.html";
            string test = "<body>< div > WHAT ??? TEST!!!!Okay, this is your new text BOI:</ div >< br />< div >{ 1}</ div ></ body > ";
            byte[] img = new HtmlToImageConverter().GenerateImage(test, NReco.ImageGenerator.ImageFormat.Jpeg);
            // var img = converter.GenerateImageFromFile(path2, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new MemoryStream(img), "test.jpg");
        }
        */

        [Command("react")]
        public async Task HandleReactionMessage()
        {
            var embed = Functions.GetDefaultBotEmbed(Functions.BotEmbedColor.Dark);
            embed.WithDescription("👿 are you gonna react to me or what?");
            RestUserMessage msg = await Context.Channel.SendMessageAsync("", embed: embed.Build());
            Global.MessageIdToTrack = msg.Id;
        }

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, [Remainder] string reason = "No reason provided.")
        {
            await user.KickAsync(reason);
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, int length, [Remainder] string reason = "No reason provided.")
        {
            await user.Guild.AddBanAsync(user, length, reason);
        }
    }
}
