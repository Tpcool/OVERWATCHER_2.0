using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class LogCommands : ModuleBase<SocketCommandContext>
    {
        private Random rndm = new Random();

        // TODO: Figure out how to do comma separated commands
        [Command("remember"), Remarks("log"),
            Summary("Posts a random message in the current server's log.")]
        public async Task Remember(string phrase = "", string user = "")
        {
            string message = string.Empty;
            // Don't allow command to initiate in DMs
            var guilds = Context.Guild;
            if (guilds == null)
            {
                message = "hey dumbass this command only works in a server";
            }
            else if (LogMessages.ServerLogMessages() == null)
            {
                message = "umm the chat logs haven't been loaded... :(";
            }
            else
            {
                IMessage msg = null;
                while (msg == null)
                {
                    List<ulong> serverMessages = new List<ulong>();
                    Dictionary<ulong, int> channelMessageCount = new Dictionary<ulong, int>();
                    foreach (SocketTextChannel channel in guilds.TextChannels)
                    {
                        List<ulong> channelMessages = LogMessages.GetChannelLog(channel.Id);
                        serverMessages.AddRange(channelMessages);
                        channelMessageCount.Add(channel.Id, channelMessages.Count);
                    }

                    if (serverMessages == null)
                    {
                        await ReplyAsync("hey dumbass this command only works in a server");
                        return;
                    }
                    else
                    {
                        int i = rndm.Next(serverMessages.Count);
                        int iTemp = i;
                        ulong channelId = 0;
                        foreach (KeyValuePair<ulong, int> chMsg in channelMessageCount)
                        {
                            if (iTemp > chMsg.Value - 1)
                            {
                                iTemp -= chMsg.Value;
                            }
                            else
                            {
                                channelId = chMsg.Key;
                            }
                        }
                        msg = await LogMessages.GetMessageUsingId(serverMessages[i], channelId);
                        if (msg != null && (msg.Content.StartsWith('.') || msg.Author.IsBot)) msg = null;
                    }
                }
                string contents = string.Empty, attachments = string.Empty;
                if (msg.Attachments != null)
                {
                    foreach (var attachment in msg.Attachments)
                    {
                        attachments += attachment.Url + "\n";
                    }
                }
                if (msg.Content != null)
                {
                    contents = msg.Content;
                }

                message = $"remember when {msg.Author.Username.ToLower()} said:\n\"{attachments}\n{contents}\"";
                if (message.Length < Constants.CharacterLimit && attachments == string.Empty)
                {
                    message = $"remember when {msg.Author.Username.ToLower()} said:\n\"{contents}\"";
                }
                else if (message.Length < Constants.CharacterLimit)
                {

                }
                else if (contents == string.Empty)
                {
                    message = $"remember when {msg.Author.Username.ToLower()} uploaded:\n\"{attachments}\"";
                }
                else if (message.Length > Constants.CharacterLimit && message.Length - attachments.Length <= Constants.CharacterLimit)
                {
                    message = $"remember when {msg.Author.Username.ToLower()} said:\n\"{contents}\"";
                }
                else
                {
                    message = contents;
                }
            }
            await ReplyAsync(message);
        }
    }
}
