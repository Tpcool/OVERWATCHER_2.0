using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBot.Utilities
{
    public static class LogMessages
    {
        // Channel ID key, list of message IDs values.
        private static Dictionary<ulong, List<ulong>> _serverLogMessages;

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
        }

        // Returns the chatlog for each channel in a dictionary.
        public static Dictionary<ulong, List<ulong>> ServerLogMessages()
        {
            return _serverLogMessages;
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
        }

        // Returns the log for the server
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
    }
}
