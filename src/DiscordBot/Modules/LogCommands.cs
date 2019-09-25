using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class LogCommands : ModuleBase<SocketCommandContext>
    {
        // create console command to fetch logs in all channels in a server
        // create a way to blacklist certain channels, like staff channels
        // fetch logs from file, but if file doesn't exist, retrieve them and then store in a file
        // have it go through the log to see if there are any recent messages to add, then append to file
        // once messages are retrieved from all channels, make the message handler add new messages to log (minus blacklisted channels)
        // .remember will find a random message through server's log, which won't have blacklisted channels, and if the message can't be found remove it from the log and search again
        // if log has not been loaded, send message
        [Command("remember"), Remarks("log"),
            Summary("Posts a random message in the current server's log.")]
        public async Task Remember(string phrase = "", string user = "")
        {
            //var msgs = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 100).FlattenAsync();
            var mess = await Context.Channel.GetMessagesAsync(500000).FlattenAsync();
            var msgList = mess.ToList();
            //foreach (var msg in MsgList)
            //{

            //}
            await ReplyAsync($"{msgList.Count} messages!");
        }
    }
}
