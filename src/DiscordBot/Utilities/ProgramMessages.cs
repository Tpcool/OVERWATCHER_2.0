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
        // Contains all retrieved messages with the channel and message data.
        private static Dictionary<IChannel, List<IMessage>> _channelMessages = new Dictionary<IChannel, List<IMessage>>();
        // Contains all retrieved messages without bot commands or bot messages.
        private static Dictionary<IChannel, List<IMessage>> _channelMessagesCondensed = new Dictionary<IChannel, List<IMessage>>();
        // Contains all of the users that messages will not be counted for in certain contexts.
        private static List<ulong> _messageBlacklist = new List<ulong>();

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

        public static List<ulong> MessageBlacklist()
        {
            return _messageBlacklist;
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

        // TODO: Make one all-purpose entry toggle function
        // If given a valid user ID, the ID will either be added or removed from the user message blacklist.
        public static void ToggleMessageBlacklistEntry(ulong id)
        {
            List<ulong> messageBlacklist = MessageBlacklist();
            // If the blacklist does not exist, create a new blacklist file with the given ID as the first entry.
            if (messageBlacklist.Count == 0 && Global.IsValidUserId(id))
            {
                messageBlacklist.Add(id);
                return;
            }

            int i = 0;
            bool wasInBlacklist = false;
            if (messageBlacklist == null) return;
            // Go through all entries and remove the ones that match, if any.
            foreach (ulong entry in messageBlacklist)
            {
                // If the channel is already in the blacklist, delist it.
                if (entry == id)
                {
                    messageBlacklist.RemoveAt(i);
                    wasInBlacklist = true;
                }
                i += 1;
            }
            // If the channel is not in the blacklist, remove any saved logs and add the ID to the blacklist.
            if (!wasInBlacklist)
            {
                RemoveMessagesIfExist();
                messageBlacklist.Add(id);
            }
            // Saves the modified blacklist to storage.
            _messageBlacklist = messageBlacklist;
            SaveBlacklistToStorage();
        }

        private static bool IsUserBlacklisted(ulong id)
        {

        }
    }
}
