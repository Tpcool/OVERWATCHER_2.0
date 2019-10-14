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

        private static bool continueConsoleTools = true;
        private static readonly char prefix = Messages.GetAlert("System.Prefix")[0];

        public static async Task ConsoleInput()
        {
            string input = "";
            ConsoleTools obj = new ConsoleTools();

            while (continueConsoleTools)
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

        [Command("exit", "Exits the command prompt features and allows the bot to listen to commands.")]
        private void ExitMethod()
        {
            LogMethod();
            continueConsoleTools = false;
        }

        [Command("log", "Makes current the list of logs and enables updates for each message received. This command is automatically called once the command prompt mode is exited.")]
        private async void LogMethod()
        {
            var context = Global.Client;
            // Go through each server and channel to see if the log needs to be created or just updated.
            foreach (SocketGuild guild in context.Guilds)
            {
                foreach (SocketTextChannel channel in guild.TextChannels)
                {
                    string consoleMessage = $"The chatlog for channel #{channel.Name} of server \"{guild.Name}\" has been ";
                    // Ignore blacklisted channels
                    if (LogMessages.IsChannelBlacklisted(channel.Id))
                    {
                        Console.WriteLine(consoleMessage + "ignored.");
                        continue;
                    }
                    // Ignore channels with no changes
                    else if (await LogMessages.IsLogUpToDate(channel))
                    {
                        Console.WriteLine(consoleMessage + "unchanged.");
                        continue;
                    }
                    // Update log
                    else if (LogMessages.DoesChannelIdExistInLog(channel.Id))
                    {
                        LogMessages.UpdateChannelMessageLog(channel);
                        Console.WriteLine(consoleMessage + "updated.");
                    }
                    // Create log
                    else
                    {
                        LogMessages.CreateChannelMessageLog(channel);
                        Console.WriteLine(consoleMessage + "created.");
                    }
                }
            }
            // Log is ready to be updated with incoming messages since it is now up-to-date.
            Global.IsLoggingActive = true;
        }

        [Command("blacklist", "Toggles the channels in the log's blacklist.")]
        private void LogBlacklistMethod(ulong id)
        {
            // Tries to store the given ID as a channel, provides feedback if it is not.
            if (Global.IsValidChannelId(id))
            {
                Console.WriteLine("Not a valid channel ID.");
                return;
            }

            // Writes the ID to a new file if it does not exist, otherwise toggles ID in current list and saves.
            LogMessages.ToggleBlacklistEntry(id);
        }

        [Command("displayblacklist", "Displays the list of channels currently being blacklisted.")]
        private void DisplayBlacklistMethod()
        {
            // Get the list and provide feedback if it is null.
            List<ulong> list = LogMessages.Blacklist();
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

        [Command("test", "For testing commands")]
        private async void TestMethodAsync()
        {

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
