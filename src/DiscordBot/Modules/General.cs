using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
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

        [Command("bux", RunMode = RunMode.Async)]
        public async Task Bux()
        {
            var account = UserAccounts.GetAccount(Context.User);
            account.Ravebux += 1;
            UserAccounts.SaveAccounts();
            var embed = Functions.GetDefaultBotEmbed();
            embed.WithDescription($"You have {account.Ravebux} ravebux. are they imaginary? {account.IsImaginary}.");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
