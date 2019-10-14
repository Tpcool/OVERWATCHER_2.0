using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class LogCommands : ModuleBase<SocketCommandContext>
    {
        private Random rndm = new Random();

        [Command("remember"), Remarks("log"),
            Summary("Posts a random message in the current server's log.")]
        public async Task Remember(string phrase = "", string user = "")
        {
            string message = string.Empty;
            //Don't allow command to initiate in DMs
            var guilds = Context.Guild;
            if (guilds == null)
            {
                message = "hey dumbass this command only works in a server";
            }
            else
            {
                List<ulong> serverMessages = new List<ulong>();
                foreach (SocketTextChannel channel in guilds.TextChannels)
                {
                    serverMessages.AddRange(LogMessages.GetChannelLog(channel.Id));
                }
                if (serverMessages == null)
                {
                    message = "hey dumbass this command only works in a server";
                }
                else
                {
                    int i = rndm.Next(serverMessages.Count);
                    // serverMessages[i];
                    // TODO: Retrieve message using its ID
                    // If the message can't be found remove it from the log and search again
                    // If log has not been loaded, send message
                    // If it's just an image or file, repost link
                }
            }

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
