using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Connection
{
    internal class ConsoleTools
    {
        public static async Task ConsoleInput()
        {
            string input = "";
            while (true)
            {
                Console.WriteLine("Awaiting console command.");
                input = Console.ReadLine().ToLower().Trim();
            }
        }

        [Command("help", "Displays a list of all commands.")]
        private void HelpMethod()
        {

        }

        [Command("log", "Creates or updates the list of logs for each server the bot is in.")]
        private void LogMethod()
        {
            // Global.Client.Guilds;
        }

        [Command("blacklist", "PARAM: Channel ID. Toggles the channels in the log's blacklist.")]
        private void LogBlacklistMethod(ulong id)
        {
            IChannel channel = Global.GetSocketChannelWithId(id);
            if (channel == null)
            {
                Console.WriteLine("Not a valid channel ID.");
                return;
            }
            string blacklist = Constants.LogBlacklist;
            string idString = id.ToString();
            if (!File.Exists(blacklist)) File.WriteAllText(blacklist, idString);
            else
            {
                var fileList = File.ReadAllText(blacklist);
                if (fileList.Contains(idString))
                {
                    fileList.Remove(fileList.IndexOf(idString), idString.Length + 1);
                    Console.WriteLine($"Channel \"{channel.Name}\" removed from blacklist.");
                }
                else
                {
                    fileList += idString + "\n";
                    File.WriteAllText(blacklist, fileList);
                    Console.WriteLine($"Channel \"{channel.Name}\" added to blacklist.");
                }
            }
        }

        [Command("displayblacklist", "Displays the list of channels currently being blacklisted.")]
        private void DisplayBlacklistMethod()
        {
            string blacklist = Constants.LogBlacklist;
            if (!File.Exists(blacklist) || File.ReadAllText(blacklist).Equals(string.Empty))
            {
                Console.WriteLine("No channels have been blacklisted.");
                return;
            }
            List<string> list = File.ReadLines(blacklist).ToList();
            string msg = "";
            foreach (string channel in list)
            {
                if (uint.TryParse(channel, out uint id))
                {
                    IChannel currentChannel = Global.GetSocketChannelWithId(id);
                    if (currentChannel != null)
                    {
                        msg += $"{id} - {currentChannel.Name}\n";
                    }
                }
            }
            if (msg.Equals(string.Empty)) Console.WriteLine("The blacklist only has invalid entries.");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class CommandAttribute : Attribute
    {
        private readonly Dictionary<string, string> _helpList = new Dictionary<string, string>();
        private readonly string _name;
        private readonly string _info;

        public CommandAttribute(string name, string info)
        {
            _name = name;
            _info = info;
            _helpList.Add(name, info);
        }
    }
}
