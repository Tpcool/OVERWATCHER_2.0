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
        private static Dictionary<ulong, List<ulong>> _serverLogMessages;
        private static List<ulong> _blacklist;

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
                // Cycle through every file in the chat log directory.
                foreach (string file in Directory.GetFiles(path))
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
                        foreach (string line in File.ReadAllLines(file))
                        {
                            ulong.TryParse(line, out ulong messageId);
                            serverMessages.Add(messageId);
                        }
                        _serverLogMessages.Add(channelId, serverMessages);
                    }
                }
            }
            // Once the logs in storage are saved, save the channel blacklist from storage.
            StoreBlacklist();
        }

        // Retrieves the blacklist from storage and saves it as a list.
        private static void StoreBlacklist()
        {
            // Return if blacklist does not exist or is empty.
            string blacklist = Constants.LogBlacklist;
            if (!File.Exists(blacklist) || File.ReadAllText(blacklist).Equals(string.Empty)) return;

            // Get the list of channel IDs, convert them to ulongs, store in list.
            List<string> stringList = File.ReadLines(blacklist).ToList();
            _blacklist = new List<ulong>(stringList.Count);
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

            foreach (ulong entry in blacklist)
            {
                blacklistString += entry.ToString() + "\n";
            }
            File.WriteAllText(blacklistPath, blacklistString);
        }

        // Saves all chatlogs stored in the dictionary to storage.
        public static void SaveLogToStorage()
        {

        }

        // Saves the given channel's chatlog in the dictionary to storage.
        public static void SaveLogToStorage(ulong channel)
        {
            string directory = Constants.LogDirectory;
            string logPath = $@"{directory}{channel.ToString()}.txt";
            if (File.Exists(logPath))
            {
                if (_serverLogMessages.TryGetValue(channel, out List<ulong> channelLog))
                {
                    string[] textLog = File.ReadAllLines(logPath);
                    if (ulong.TryParse(textLog[textLog.Length - 1], out ulong mostRecentSavedMessage))
                    {
                        int i = channelLog.IndexOf(mostRecentSavedMessage);
                    }
                }
            }
            else
            {

            }
        }

        public static void SaveLogToStorage(ulong channel, ulong message)
        {
            string directory = Constants.LogDirectory;
            string logPath = $@"{directory}{channel.ToString()}.txt";
            if (File.Exists(logPath))
            {
                if (_serverLogMessages.TryGetValue(channel, out List<ulong> channelLog))
                {
                    int i = channelLog.IndexOf();
                }
            }
            else
            {

            }
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
            foreach (ulong savedId in _serverLogMessages.Keys)
            {
                if (id == savedId) return true;
            }
            return false;
        }

        // Adds a channel ID and list of message IDs to the dictionary for the current chatlog.
        public static void AddOrUpdateChannelLog(ulong channel, List<ulong> messages)
        {
            if (Global.GetSocketGuildWithId(channel) == null) return;
            // Checks to see if the log already exists for the channel, and if it does, overwrites the list.
            foreach (ulong logChannel in _serverLogMessages.Keys)
            {
                if (channel == logChannel)
                {
                    _serverLogMessages[channel] = messages;
                }
            }
            _serverLogMessages.Add(channel, messages);
            // TODO: save log to storage?
        }

        // Returns the log (list of message IDs) for the given channel
        public static List<ulong> GetChannelLog(ulong channel)
        {
            foreach (ulong id in _serverLogMessages.Keys)
            {
                if (channel == id)
                {
                    if (_serverLogMessages.TryGetValue(id, out List<ulong> log))
                    {
                        return log;
                    }
                }
            }
            return null;
        }

        // Returns a list of all message IDs saved in a single list that share the same server ID.
        public static uint GetServerMessageCount(ulong server)
        {
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
            // Go through every entry and compare it to the given channel ID.
            foreach (ulong blacklistedChannel in blacklist)
            {
                if (channel == blacklistedChannel) return true;
            }
            return false;
        }

        public static async Task<List<IMessage>> GetListOfChannelMessages(SocketTextChannel channel, int numMessages)
        {
            List<IMessage> messageList = (await channel.GetMessagesAsync(numMessages).FlattenAsync()).ToList();
            return messageList;
        }

        public static async Task<List<IMessage>> GetListOfChannelMessages(SocketTextChannel channel, int numMessages, IMessage startingMessage, Direction direction)
        {
            List<IMessage> messageList = (await channel.GetMessagesAsync(startingMessage, direction, numMessages).FlattenAsync()).ToList();
            return messageList;
        }
    }
}
