using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    public class GeneralCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _cmd;

        public GeneralCommands(CommandService cmd)
        {
            _cmd = cmd;
        }

        [Command("help"), Remarks("general"),
            Summary("Displays the list of commands, or the use of a specific command.")]
        public async Task Help([Remainder]string command = "")
        {
            StringBuilder commandList = new StringBuilder(Constants.CharacterLimit);
            char prefix = Messages.GetAlert("Command.Prefix")[0];
            var commands = Functions.GetHelpList(_cmd);
            command = command.ToLower().Trim().Trim(prefix);

            if (!command.Equals("")) // Handles the command with a parameter.
            {
                foreach (var entry in commands) 
                {
                    if (entry.name.Equals(command)) // Add description of command if a match is found.
                    {
                        commandList.Append($"`{prefix}{entry.name} {entry.parameters}` {entry.category} command\n{entry.description}");
                        break;
                    }
                }
                if (commandList.Equals("")) // Add message if the command does not exist.
                {
                    commandList.Append($"The `{prefix}{command}` command could not be found.");
                }
            }
            else // Handles if the command is entered with no parameters.
            {
                string previousCategory = "";
                commands = commands.OrderBy(c => c.category).ToList();
                foreach (var entry in commands)
                {
                    if (!previousCategory.Equals(entry.category)) commandList.Append($"\n\n{entry.category} commands:\n"); // Set up new section for that command type.
                    commandList.Append($"`{prefix}{entry.name}` ");
                    previousCategory = entry.category;
                }
                commandList.Append($"\n\nEnter `{prefix}help [command]*` for more info. 😈 Optional parameters are marked with an asterisk.");
            }

            await Context.Channel.SendMessageAsync(commandList.ToString());
        }

    }
}
