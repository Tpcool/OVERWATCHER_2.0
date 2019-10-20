using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DiscordBot.Utilities.LogMessages;

namespace DiscordBot.Utilities
{
    public static class ProgramMessages
    {
        // Contains all retrieved messages with the channel and message data.
        private static Dictionary<IChannel, List<IMessage>> _channelMessages = new Dictionary<IChannel, List<IMessage>>();
        // Contains all retrieved messages without bot commands or bot messages.
        private static Dictionary<IChannel, List<IMessage>> _channelMessagesCondensed = new Dictionary<IChannel, List<IMessage>>();
        // Contains all of the users that messages will not be counted for in certain contexts.
        private static List<ulong> _userBlacklist = new List<ulong>();

        static ProgramMessages()
        {
            SetUpProgramMessages();
        }

        private static async void SetUpProgramMessages()
        {
            Dictionary<ulong, List<ulong>> serverLogMessages = ServerLogMessages();
            foreach (KeyValuePair<ulong, List<ulong>> entry in serverLogMessages)
            {
                SocketTextChannel channel = Global.GetSocketChannelWithId(entry.Key) as SocketTextChannel;
                List<IMessage> tempChannelMessages = new List<IMessage>(entry.Value.Count);
                List<IMessage> tempChannelMessagesCondensed = new List<IMessage>(entry.Value.Count);

                foreach (ulong msgId in entry.Value)
                {
                    IMessage msg = await GetMessageUsingId(msgId, channel);
                    if (msg == null) continue;

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

        public static List<ulong> UserBlacklist()
        {
            return _userBlacklist;
        }

        public static Dictionary<IChannel, List<IMessage>> ChannelMessages()
        {
            return _channelMessages;
        }

        public static Dictionary<IChannel, List<IMessage>> ChannelMessagesCondensed()
        {
            return _channelMessagesCondensed;
        }

        public static List<IMessage> GetSavedChannelMessages(IChannel channel)
        {
            if (_channelMessages.TryGetValue(channel, out List<IMessage> list))
            {
                return list;
            }
            else
            {
                return null;
            }
        }

        public static List<IMessage> GetSavedChannelMessagesCondensed(IChannel channel)
        {
            if (_channelMessagesCondensed.TryGetValue(channel, out List<IMessage> list))
            {
                return list;
            }
            else
            {
                return null;
            }
        }

        private static async Task<IMessage> GetMessageUsingId(ulong messageId, SocketTextChannel channel)
        {
            IMessage msg = await channel.GetMessageAsync(messageId);
            return msg ?? null;
        }

        private static bool IsValidMessage(IMessage msg)
        {
            // Is the author of the message blacklisted?
            bool userBlacklisted = DoesValueExistInList(_userBlacklist, msg.Author.Id);
            // Is the message one that is invoking a bot command?
            bool commandMessage = msg.Content.StartsWith(Messages.GetAlert("System.Prefix")[0]);
            if (userBlacklisted || commandMessage)
            {
                return false;
            }
            return true;
        }
        
        // If given a valid user ID, the ID will either be added or removed from the user message blacklist.
        public static void ToggleUserBlacklistEntry(ulong id)
        {
            string path = Constants.UserBlacklist;
            FileReturn status = ToggleStorageEntry(path, id.ToString());
            if (status == FileReturn.FileDoesNotExist)
            {
                return;
            }
            else if (status == FileReturn.EntryAdded)
            {
                // TODO: Remove all instances of user in condensed log
            }

            // Gets the modified log from storage and sets it to the variable.
            List<ulong> userBlacklist = GetListFromStorage(path);
            _userBlacklist = userBlacklist;
        }
    }
}
