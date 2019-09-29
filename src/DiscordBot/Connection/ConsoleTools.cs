using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DiscordBot.Connection
{
    internal class ConsoleTools
    {
        public static async Task ConsoleInput()
        {
            string input = "";
            ConsoleTools obj = new ConsoleTools();

            while (true)
            {
                input = Console.ReadLine().ToLower().Trim();
                if (!input.StartsWith('.') && !(input.Length > 1)) continue;
                else input = input.Substring(1);

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
            if (input.Contains(' ')) input = input.Substring(input.IndexOf(' '));
            else return new object[] { };
            string[] list = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < list.Length; i++) list[i] = list[i].Trim();
            return list;
        }

        private object[] ConvertParams(object[] parameters, MethodInfo method)
        {
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
            var methods = typeof(ConsoleTools).GetRuntimeMethods();
            foreach (MethodInfo method in methods)
            {
                foreach (Attribute attr in method.GetCustomAttributes())
                {
                    if (attr is CommandAttribute cmd)
                    {
                        Console.WriteLine($".{cmd.Name} - {cmd.Info}");
                    }
                }
            }
        }

        [Command("log", "Creates or updates the list of logs for each server the bot is in.")]
        private void LogMethod(int a, ulong b, char c)
        {
            // Global.Client.Guilds;
            Console.WriteLine(a + " " + b + " " + c);
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

        [Command("block", "Toggles the bot on/off so it can only listen to console commands.")]
        private void BlockCommand()
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
