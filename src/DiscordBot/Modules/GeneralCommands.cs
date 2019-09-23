using System.Collections;
using System.Collections.Generic;
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
        private CommandService _cmd;

        public GeneralCommands(CommandService cmd)
        {
            _cmd = cmd;
        }

        [Command("help"), Remarks("general"),
            Summary("Displays the list of commands, or the use of a specific command.")]
        public async Task Help([Remainder]string command = "")
        {
            StringBuilder commandList = new StringBuilder(Constants.CharacterLimit);
            var commands = Functions.GetHelpList(_cmd);
            command = command.ToLower().Trim();
            if (command.StartsWith('.')) command = command.Substring(1); // todo: replace hardcoded command prefix

            if (!command.Equals("")) //todo: condense ifelse branch?
            {
                foreach (var entry in commands)
                {
                    if (entry.name.Equals(command))
                    {
                        commandList.Append($"{entry.category} command\n`.{entry.name} {entry.parameters}`\n{entry.description}");
                        break;
                    }
                }
                if (commandList.Equals(""))
                {
                    commandList.Append($"The .{command} command could not be found.");
                }
            }
            else
            {
                foreach (var entry in commands)
                {
                    commandList.Append($"{entry.category}: .{entry.name}\n");
                }
            }

            await Context.Channel.SendMessageAsync(commandList.ToString());
        }

    }
}
