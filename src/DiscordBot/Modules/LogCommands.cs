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
            else if (ProgramMessages.ChannelMessagesCondensed() == null)
            {
                message = "umm the chat logs haven't been loaded... :(";
            }
            else
            {
                IMessage msg = null;
                List<IMessage> serverMessages = new List<IMessage>();
                Dictionary<SocketTextChannel, int> channelMessageCount = new Dictionary<SocketTextChannel, int>();
                foreach (SocketTextChannel channel in guilds.TextChannels)
                {
                    List<IMessage> channelMessages = ProgramMessages.GetSavedChannelMessagesCondensed(channel);
                    if (channelMessages != null)
                    {
                        serverMessages.AddRange(channelMessages);
                        channelMessageCount.Add(channel, channelMessages.Count);
                    }
                }
                if (serverMessages == null)
                {
                    await ReplyAsync("hey dumbass this command only works in a server");
                    return;
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
