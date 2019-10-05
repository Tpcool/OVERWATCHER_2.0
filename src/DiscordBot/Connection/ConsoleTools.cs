using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DiscordBot.Utilities;
using Discord.WebSocket;

namespace DiscordBot.Connection
{
    internal class ConsoleTools
    {
        private static readonly char prefix = Messages.GetAlert("System.Prefix")[0];

        public static async Task ConsoleInput()
        {
            string input = "";
            ConsoleTools obj = new ConsoleTools();

            while (true)
            {
                // Retrieves input and, if it is a command, attempts to format it to be parsed.
                input = Console.ReadLine().ToLower().Trim();
                if (!input.StartsWith(prefix) || !(input.Length > 1)) continue;
                else input = input.Substring(1);

                // Gets parameters and methods, and attempts to invoke a method if it matches the inputted command.
                var parameters = obj.GetParams(input);
                var methods = typeof(ConsoleTools).GetRuntimeMethods();
                foreach (MethodInfo method in methods)
                {
                    foreach (Attribute attr in method.GetCustomAttributes())
                    {
                        if (attr is CommandAttribute cmd && input.StartsWith(cmd.Name))
                        {
                            parameters = obj.ConvertParams(parameters, method);
                            try
                            {
                                method.Invoke(obj, parameters);
                                continue;
                            }
                            catch (TargetParameterCountException)
                            {
                                Console.WriteLine("Invalid paramters.");
                            }
                        }
                    }
                }
            }
        }

        private object[] GetParams(string input)
        {
            // Isolates the parameters by removing the command and saving the split and trimmed input to an array.
            if (input.Contains(' ')) input = input.Substring(input.IndexOf(' '));
            else return new object[] { };
            string[] list = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < list.Length; i++) list[i] = list[i].Trim();
            return list;
        }

        private object[] ConvertParams(object[] parameters, MethodInfo method)
        {
            // Attempts to convert the inputted objects into each of the method's data types.
            int i = 0;
            object[] newParams = new object[parameters.Length];
            foreach (var param in method.GetParameters())
            {
                if (i >= parameters.Length) return parameters;
                try
                {
                    newParams[i] = Convert.ChangeType(parameters[i], param.ParameterType);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to convert parameter \"{parameters[i]}\" to {param.ParameterType}.");
                }
                i += 1;
            }
            return newParams;
        }

        [Command("help", "Displays a list of all commands.")]
        private void HelpMethod()
        {
            // Cycles through all methods and locates the attribute that contains the command info to display.
            var methods = typeof(ConsoleTools).GetRuntimeMethods();
            foreach (MethodInfo method in methods)
            {
                string methodParams = "";
                foreach (Attribute attr in method.GetCustomAttributes())
                {
                    if (attr is CommandAttribute cmd)
                    {
                        foreach (ParameterInfo param in method.GetParameters())
                        {
                            methodParams += param.Name + ",";
                        }
                        methodParams = methodParams.TrimEnd(',');
                        Console.WriteLine($"{prefix}{cmd.Name} [{methodParams}] - {cmd.Info}");
                    }
                }
            }
        }

        [Command("block", "Toggles the bot on/off so it can only listen to console commands.")]
        private void BlockMethod()
        {

        }

        // Create directory if it does not exist
        // Extract and store previous logs in dictionary if they exist
        // Cycle through servers
        // Add servers that are absent
        // Update servers that are not up-to-date
        [Command("log", "Makes current the list of logs and enables updates for each message received.")]
        private async void LogMethod()
        {
            var context = Global.Client;
            // Go through each server to see if the log needs to be created or just updated.
            foreach (SocketGuild guild in context.Guilds)
            {
                // Update log
                if (LogMessages.DoesChannelIdExistInLog(guild.Id))
                {

                }
                // Create log
                else
                {
                    LogMessages.AddChannelLog(guild.Id, await GetAllServerMessages(guild));
                }
            }

            // Log is ready to be updated with incoming messages since it is now up-to-date.
            Global.IsLoggingActive = true;
        }

        private async Task<List<ulong>> GetAllServerMessages(SocketGuild guild)
        {
            const int messagesToRetrieve = 500000;
            List<ulong> messageIds = new List<ulong>();

            foreach (SocketTextChannel channel in guild.TextChannels)
            {
                var messageList = (await channel.GetMessagesAsync(messagesToRetrieve).FlattenAsync()).ToList();
                foreach (SocketMessage message in messageList)
                {
                    messageIds.Add(message.Id);
                }
            }
            return messageIds;
        }

        private async Task<List<ulong>> GetRemainingServerMessages(SocketGuild guild)
        {
            // Consider putting each channel in its own log file???
            // Must account for deleted messages, new channels, empty channels.
            const int messagesToRetrieve = 50;
            List<ulong> serverMessages = LogMessages.GetChannelLog(guild.Id);
            foreach (SocketTextChannel channel in guild.TextChannels)
            {
                var messageList = (await channel.GetMessagesAsync(messagesToRetrieve).FlattenAsync()).ToList();
                // if statement to check to see if the server message is in the current channel

                for (int i = messageList.Count; i >= 0; i--)
                {
                    if (messageList.ElementAt(i).Id == message.Id)
                    {
                        for (int j = i + 1; j < messageList.Count(); j++)
                        {
                            serverMessages.Add(messageList.ElementAt(j).Id);
                        }
                        break;
                    }
                }

            }

            return null;
        }

        [Command("blacklist", "Toggles the channels in the log's blacklist.")]
        private void LogBlacklistMethod(ulong id)
        {
            // Tries to store the given ID as a channel, provides feedback if it is not.
            IChannel channel = Global.GetSocketChannelWithId(id);
            if (channel == null)
            {
                Console.WriteLine("Not a valid channel ID.");
                return;
            }

            // Writes the ID to a new file if it does not exist, otherwise toggles ID in current list and saves.
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
            // Get the list and provide feedback if it is null.
            List<ulong> list = GetBlacklist();
            if (list == null)
            {
                Console.WriteLine("Blacklist is empty or does not exist.");
                return;
            }

            // Cycles through each channel and finds valid info to display.
            string msg = "";
            foreach (uint id in list)
            {
                IChannel currentChannel = Global.GetSocketChannelWithId(id);
                if (currentChannel != null) msg += $"{id} - {currentChannel.Name}\n";
            }
            if (msg.Equals(string.Empty)) msg = "The blacklist only has invalid entries.";
            Console.WriteLine(msg);
        }

        private List<ulong> GetBlacklist()
        {
            // Return a null list if it does not exist or is empty.
            string blacklist = Constants.LogBlacklist;
            if (!File.Exists(blacklist) || File.ReadAllText(blacklist).Equals(string.Empty)) return null;

            // Get the list of channel IDs, convert them to ulongs, store in list.
            List<string> stringList = File.ReadLines(blacklist).ToList();
            List<ulong> channelList = new List<ulong>(stringList.Count);
            foreach (string channel in stringList)
            {
                if (ulong.TryParse(channel, out ulong id)) channelList.Add(id);
            }
            return channelList;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class CommandAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Info;

        public CommandAttribute(string name, string info)
        {
            Name = name;
            Info = info;
        }
    }
}
