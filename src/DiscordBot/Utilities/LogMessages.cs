using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Utilities
{
    public static class LogMessages
    {
        // Channel ID key, list of message IDs values.
        private static Dictionary<ulong, List<ulong>> _serverLogMessages = new Dictionary<ulong, List<ulong>>();
        private static List<ulong> _blacklist;
        private const int NewMessagesToRetrieve = 500000;
        private const int UpdateMessagesToRetrieve = 100;

        // Opens and stores the chat log IDs.
        static LogMessages()
        {
            string path = Constants.LogDirectory;

            // Create blank directory to eventually populate with log files.
            if (!Directory.Exists(Constants.LogDirectory))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                // Save the channel blacklist from storage.
                LoadBlacklist();
                // Cycle through every file in the chat log directory.
                string[] allFiles = Directory.GetFiles(path);
                if (allFiles == null) return;
                foreach (string file in allFiles)
                {
                    // Split the filename into its individual components.
                    string[] serverStringSplit = file.Split('\\');
                    // Get the filename without the path.
                    string serverString = serverStringSplit[serverStringSplit.Length - 1];
                    // Get the filename without the file type, leaving just the channel ID.
                    serverString = serverString.Substring(0, serverString.IndexOf('.'));
                    // Converts and stores all message IDs in a list, then adds server/channel and its messages to the dictionary.
                    var serverMessages = new List<ulong>();
                    if (ulong.TryParse(serverString, out ulong channelId))
                    {
                        string[] lines = File.ReadAllLines(file);
                        if (!(lines == null))
                        {
                            foreach (string line in lines)
                            {
                                ulong.TryParse(line, out ulong messageId);
                                serverMessages.Add(messageId);
                            }
                            _serverLogMessages.Add(channelId, serverMessages);
                        }
                    }
                }
            }
        }

        // Retrieves the blacklist from storage and saves it as a list.
        private static void LoadBlacklist()
        {
            // Return if blacklist does not exist or is empty.
            string blacklist = Constants.LogBlacklist;
            if (!File.Exists(blacklist) || File.ReadAllText(blacklist).Equals(string.Empty)) return;

            // Get the list of channel IDs, convert them to ulongs, store in list.
            List<string> stringList = File.ReadLines(blacklist).ToList();
            _blacklist = new List<ulong>(stringList.Count);
            if (stringList == null) return;
            foreach (string channel in stringList)
            {
                if (ulong.TryParse(channel, out ulong id)) _blacklist.Add(id);
            }
        }

        // Returns the chatlog for each channel in a dictionary.
        public static Dictionary<ulong, List<ulong>> ServerLogMessages()
        {
            return _serverLogMessages;
        }

        public static List<ulong> Blacklist()
        {
            return _blacklist;
        }

        // If given a valid channel ID, the ID will either be added or removed from the channel blacklist.
        public static void ToggleBlacklistEntry(ulong id)
        {
            List<ulong> blacklist = Blacklist();
            // If the blacklist does not exist, create a new blacklist file with the given ID as the first entry.
            if (blacklist.Count == 0 && Global.IsValidChannelId(id))
            {
                blacklist.Add(id);
                return;
            }
            IChannel channel = Global.GetSocketChannelWithId(id);

            int i = 0;
            bool wasInBlacklist = false;
            if (blacklist == null) return;
            // Go through all entries and remove the ones that match, if any.
            foreach (ulong entry in blacklist)
            {
                // If the channel is already in the blacklist, delist it.
                if (entry == id)
                {
                    blacklist.RemoveAt(i);
                    wasInBlacklist = true;
                }
                i += 1;
            }
            // If the channel is not in the blacklist, remove any saved logs and add the ID to the blacklist.
            if (!wasInBlacklist)
            {
                RemoveLogIfExists(id);
                blacklist.Add(id);
            }
            // Saves the modified blacklist to storage.
            _blacklist = blacklist;
            SaveBlacklistToStorage();
        }

        // Cycles through all entries in the blacklist data structure and saves it to storage.
        private static void SaveBlacklistToStorage()
        {
            string blacklistPath = Constants.LogBlacklist;
            string blacklistString = string.Empty;
            List<ulong> blacklist = Blacklist();

            if (blacklist == null) return;
            foreach (ulong entry in blacklist)
            {
                blacklistString += entry.ToString() + "\n";
            }
            File.WriteAllText(blacklistPath, blacklistString);
        }

        // Saves the given channel's chatlog in the dictionary to storage using only the new messages in that channel.
        public static void AppendLogToStorage(ulong channel, List<ulong> newMessages)
        {
            if (newMessages == null) return;
            string directory = Constants.LogDirectory;
            string logPath = $@"{directory}{channel.ToString()}.txt";
            if (File.Exists(logPath))
            {
                using (FileStream logFile = new FileStream(logPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(logFile))
                    {
                        foreach (ulong msg in newMessages)
                        {
                            sw.WriteLine(msg.ToString());
                        }
                    }
                }
            }
            else
            {
                string messages = string.Empty;
                foreach (ulong msg in newMessages)
                {
                    messages += msg.ToString() + "\n";
                }
                File.WriteAllText(logPath, messages);
            }
        }

        public static void AppendLogToStorage(ulong channel, ulong message)
        {
            AppendLogToStorage(channel, new List<ulong> { message });
        }

        // Checks to see if the log exists in storage, and deletes it if it does.
        public static void RemoveLogIfExists(ulong channel)
        {
            string directory = Constants.LogDirectory;
            string logPath = $@"{directory}{channel.ToString()}.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
        }

        // Checks to see if the channel ID specified exists in the current chatlog.
        public static bool DoesChannelIdExistInLog(ulong id)
        {
            if (_serverLogMessages == null) return false;
            foreach (ulong savedId in _serverLogMessages.Keys)
            {
                if (id == savedId) return true;
            }
            return false;
        }

        // Adds a channel ID and list of message IDs to the dictionary for the current chatlog.
        public static void AddOrUpdateChannelLog(ulong channel, List<ulong> messages)
        {
            if (!Global.IsValidChannelId(channel)) return;
            // Checks to see if the log already exists for the channel, and if it does, overwrites the list.
            if (_serverLogMessages != null && _serverLogMessages.ContainsKey(channel))
            {
                _serverLogMessages[channel] = messages;
            }
            else
            {
                _serverLogMessages.Add(channel, messages);
            }
        }

        // Adds a single message to the channel log.
        public static void AddOrAppendChannelLog(ulong channel, ulong message)
        {
            if (!Global.IsValidChannelId(channel)) return;
            if (_serverLogMessages.ContainsKey(channel))
            {
                _serverLogMessages[channel].Add(message);
            }
            else
            {
                _serverLogMessages.Add(channel, new List<ulong> { message });
            }
        }

        // Returns the log (list of message IDs) for the given channel
        public static List<ulong> GetChannelLog(ulong channel)
        {
            if (_serverLogMessages.TryGetValue(channel, out List<ulong> log))
            {
                return log;
            }
            return null;
        }

        // Returns a list of all message IDs saved in a single list that share the same server ID.
        public static uint GetServerMessageCount(ulong server)
        {
            if (_serverLogMessages == null) return 0;
            // Return if the server is invalid.
            if (Global.GetSocketGuildWithId(server) == null)
            {
                Console.WriteLine("Server does not exist in the current context.");
                return 0;
            }
            uint serverMessageCount = 0;
            // Iterate through each channel in the log.
            foreach (ulong channel in _serverLogMessages.Keys)
            {
                // If the channel's server is equal to the specified server, add the list's count to the total.
                SocketGuild guild = Global.GetSocketGuildWithChannelId(channel);
                if (guild.Id == server)
                {
                    if (_serverLogMessages.TryGetValue(channel, out List<ulong> logs))
                    {
                        serverMessageCount += (uint)logs.Count;
                    }
                }
            }
            return serverMessageCount;
        }

        // Checks to see if the received channel ID is in the list of blacklisted channels.
        public static bool IsChannelBlacklisted(ulong channel)
        {
            List<ulong> blacklist = Blacklist();
            if (blacklist == null) return false;
            // Go through every entry and compare it to the given channel ID.
            foreach (ulong blacklistedChannel in blacklist)
            {
                if (channel == blacklistedChannel) return true;
            }
            return false;
        }

        // Returns true if the given channel has a log that does not have any new messages compared to what is already stored.
        public static async Task<bool> IsLogUpToDate(SocketTextChannel channel)
        {
            List<ulong> channelMessages = GetChannelLog(channel.Id);
            List<IMessage> msg = await GetListOfChannelMessages(channel, 1);
            if (channelMessages == null || msg == null) return false;
            // Retrieve the newest message from the log and compare it with the newest message in the chat.
            ulong mostRecentMessageId = channelMessages[channelMessages.Count - 1];
            if (mostRecentMessageId == msg[0].Id) return true;
            return false;
        }

        // Updates the chatlog for all channels in a server if they exist.
        public static async void UpdateChannelMessageLog(SocketTextChannel channel)
        {
            List<ulong> channelMessages = GetChannelLog(channel.Id);
            List<ulong> newMessages = await GetUpdatedChannelMessages(channel, channelMessages);
            channelMessages.AddRange(newMessages);
            AddOrUpdateChannelLog(channel.Id, channelMessages);
            AppendLogToStorage(channel.Id, newMessages);
        }

        // Sets the chatlog for the given channel.
        public static async void CreateChannelMessageLog(SocketTextChannel channel)
        {
            List<ulong> messageIds = await GetAllChannelMessages(channel);
            AddOrUpdateChannelLog(channel.Id, messageIds);
            AppendLogToStorage(channel.Id, messageIds);
        }

        // Returns a list of all message IDs in the specified channel.
        private static async Task<List<ulong>> GetAllChannelMessages(SocketTextChannel channel)
        {
            var messageList = await GetListOfChannelMessages(channel, NewMessagesToRetrieve);
            List<ulong> messageIds = new List<ulong>(messageList.Count);

            foreach (IMessage msg in messageList)
            {
                messageIds.Add(msg.Id);
            }
            return messageIds;
        }

        // Returns a list of all of the new message IDs in a channel.
        private static async Task<List<ulong>> GetUpdatedChannelMessages(SocketTextChannel channel, List<ulong> oldMessages)
        {
            if (oldMessages == null) return null;
            long mostRecentTimestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds();

            // Goes through all currently stored messages to find the most recent valid entry to save when it was posted.
            for (int i = oldMessages.Count - 1; i >= 0; i--)
            {
                IMessage msg = await channel.GetMessageAsync(oldMessages[i]);
                if (!(msg == null))
                {
                    mostRecentTimestamp = msg.CreatedAt.ToUnixTimeMilliseconds();
                    break;
                }
            }

            // If no valid entries were found, provide feedback and return nothing.
            if (mostRecentTimestamp == DateTimeOffset.MaxValue.ToUnixTimeMilliseconds())
            {
                Console.WriteLine($"Cannot locate most recent message in {channel.Name}'s log.");
                return null;
            }

            // Cycles through all new messages until the most recently saved message is found and saves all new entries to a list.
            List<IMessage> newMessages = await GetListOfChannelMessages(channel, UpdateMessagesToRetrieve);
            List<IMessage> updatedMessages = new List<IMessage>();
            bool isCurrentMessageReached = false;
            while (!isCurrentMessageReached && newMessages.Count > 1)
            {
                newMessages.Reverse(); // Have messages sorted from newest to oldest.
                for (int i = 0; i < newMessages.Count; i++)
                {
                    if (mostRecentTimestamp == newMessages[i].CreatedAt.ToUnixTimeMilliseconds())
                    {
                        isCurrentMessageReached = true;
                        updatedMessages = await GetListOfChannelMessages(channel, NewMessagesToRetrieve, newMessages[i], Direction.After);
                        break;
                    }
                }
                if (!isCurrentMessageReached)
                {
                    newMessages = await GetListOfChannelMessages(channel, NewMessagesToRetrieve, newMessages[newMessages.Count - 1], Direction.Before);
                }
            }

            // Create a list with only the message IDs of the new messages and return it.
            List<ulong> newMessageIds = new List<ulong>(updatedMessages.Count);
            foreach (var msg in updatedMessages)
            {
                newMessageIds.Add(msg.Id);
            }
            return newMessageIds;
        }

        // Returns a list of channel messages from oldest to newest.
        public static async Task<List<IMessage>> GetListOfChannelMessages(SocketTextChannel channel, int numMessages)
        {
            List<IMessage> messageList = (await channel.GetMessagesAsync(numMessages).FlattenAsync()).ToList();
            messageList.Reverse();
            return messageList;
        }

        // Returns a list of channel messages from oldest to newest using a starting message and direction.
        public static async Task<List<IMessage>> GetListOfChannelMessages(SocketTextChannel channel, int numMessages, IMessage startingMessage, Direction direction)
        {
            List<IMessage> messageList = (await channel.GetMessagesAsync(startingMessage, direction, numMessages).FlattenAsync()).ToList();
            messageList.Reverse();
            return messageList;
        }
    }
}
