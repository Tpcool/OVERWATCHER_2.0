using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot.Utilities
{
    public static class LogMessages
    {
        // Actually, maybe just redesign to have the dict only take channel IDs since guilds can be inferred from them??
        public struct ServerChannel
        {
            public ulong ServerId;
            public ulong ChannelId;
        }
        private static Dictionary<ServerChannel, List<ulong>> _serverLogMessages;

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
                    // Isolate server ID by deconstructing its filename.
                    string[] serverStringSplit = file.Split('\\');
                    string serverString = serverStringSplit[serverStringSplit.Length - 1];
                    serverString = serverString.Substring(0, serverString.IndexOf('.'));
                    string[] stringServerChannel = serverString.Split('-');
                    ServerChannel logServerChannel;
                    // Converts and stores all message IDs in a list, then adds server/channel and its messages to the dictionary.
                    var serverMessages = new List<ulong>();
                    if (ulong.TryParse(stringServerChannel[0], out logServerChannel.ServerId) && 
                        ulong.TryParse(stringServerChannel[stringServerChannel.Length - 1], out logServerChannel.ChannelId))
                    {
                        foreach (string line in File.ReadAllLines(file))
                        {
                            ulong.TryParse(line, out ulong messageId);
                            serverMessages.Add(messageId);
                        }
                        _serverLogMessages.Add(logServerChannel, serverMessages);
                    }
                }
            }
        }

        public static Dictionary<ulong, List<ulong>> ServerLogMessages()
        {
            return _serverLogMessages;
        }

        public static bool DoesServerIdExist(ulong id)
        {
            foreach (ulong savedId in _serverLogMessages.Keys)
            {
                if (id == savedId) return true;
            }
            return false;
        }

        public static void AddServerLog(ulong server, List<ulong> messages)
        {
            if (Global.GetSocketGuildWithId(server) == null) return;
            _serverLogMessages.Add(server, messages);
        }

        // Returns the log for the server
        public static List<ulong> GetServerLog(ulong server)
        {
            foreach (ulong id in _serverLogMessages.Keys)
            {
                if (server == id)
                {
                    if (_serverLogMessages.TryGetValue(id, out List<ulong> log))
                    {
                        return log;
                    }
                }
            }
            return null;
        }
    }
}
