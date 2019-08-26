using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

            if (Functions.UserHasRole((SocketGuildUser)Context.User, role))
            {
                await dm.SendMessageAsync(FormatMessages.GetFormattedAlert("TestRoleMsg_&ROLE", role));
            }
            else
            {
                await dm.SendMessageAsync(FormatMessages.GetFormattedAlert("TestNoRoleMsg_&ROLE", role));
            }
        }

        [Command("bigtest", RunMode = RunMode.Async)]
        public async Task Bigtest()
        {
            
        }
    }
}
