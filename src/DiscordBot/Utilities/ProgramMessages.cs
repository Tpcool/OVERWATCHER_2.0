using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utilities
{
    public class ProgramMessages
    {
        // Contains all retrieved messages with the channel and message data
        private static Dictionary<IChannel, List<IMessage>> _channelMessages = new Dictionary<IChannel, List<IMessage>>();
        // Contains all retrieved messages without bot commands or bot messages.
        private static Dictionary<IChannel, List<IMessage>> _channelMessagesCondensed = new Dictionary<IChannel, List<IMessage>>();

        static ProgramMessages()
        {
            SetUpProgramMessages();
        }

        private static async void SetUpProgramMessages()
        {
            Dictionary<ulong, List<ulong>> serverLogMessages = LogMessages.ServerLogMessages();
            foreach (KeyValuePair<ulong, List<ulong>> entry in serverLogMessages)
            {
                SocketTextChannel channel = Global.GetSocketChannelWithId(entry.Key) as SocketTextChannel;
                List<IMessage> tempChannelMessages = new List<IMessage>(entry.Value.Count);
                List<IMessage> tempChannelMessagesCondensed = new List<IMessage>(entry.Value.Count);

                foreach (ulong msgId in entry.Value)
                {
                    IMessage msg = await GetMessageUsingId(msgId, channel);

                    tempChannelMessages.Add(msg);
                    if (IsValidMessage(msg)) tempChannelMessagesCondensed.Add(msg);
                }

                if (!_channelMessages.TryAdd(channel, tempChannelMessages))
                {
                    Console.WriteLine($"Could not add channel \"{channel.Name}\" to either log.");
                }
                else if (!_channelMessagesCondensed.TryAdd(channel, tempChannelMessagesCondensed))
                {
                    Console.WriteLine($"Could not add channel \"{channel.Name}\" to condensed log.");
                }
            }
        }

        private static async Task<IMessage> GetMessageUsingId(ulong messageId, SocketTextChannel channel)
        {
            IMessage msg = await channel.GetMessageAsync(messageId);
            return msg ?? null;
        }

        private static bool IsValidMessage(IMessage msg)
        {
            return false;
        }
    }
}
