using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot.Utilities
{
    public static class LogMessages
    {
        private static Dictionary<ulong, List<ulong>> _serverLogMessages;

        // Opens and stores the chat log IDs.
        static LogMessages()
        {
            string path = Constants.LogDirectory;

            if (!Directory.Exists(Constants.LogDirectory)) Directory.CreateDirectory(path);
            else
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    // Isolate server ID by deconstructing its filename.
                    string[] serverStringSplit = file.Split('\\');
                    string serverString = serverStringSplit[serverStringSplit.Length - 1];
                    serverString = serverString.Substring(0, serverString.IndexOf('.'));
                    // Converts and stores all message IDs in a list, then adds server and its messages to the dictionary.
                    var serverMessages = new List<ulong>();
                    if (ulong.TryParse(serverString, out ulong serverId))
                    {
                        foreach (string line in File.ReadAllLines(file))
                        {
                            ulong.TryParse(line, out ulong messageId);
                            serverMessages.Add(messageId);
                        }
                    }
                    _serverLogMessages.Add(serverId, serverMessages);
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
